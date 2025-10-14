using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ManyBox.Utils;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace ManyBox.Components.Pages.Admin
{
    public partial class Rutas
    {
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private List<PaqueteAsignadoModel> PaquetesAsignados = new();
        private List<EmpleadoModel> Empleados = new();
        private List<VentaModel> VentasDisponibles = new();
        private string EmpleadoSeleccionadoId = string.Empty;
        private string VentaSeleccionadaId = string.Empty;
        private string GuiaRastreo = string.Empty;
        private bool UsuarioEsAdmin = false;
        private DateTime? FechaEntregaAsignar = null;

        // Modal edición fecha
        private bool ModalFechaVisible = false;
        private PaqueteAsignadoModel? PaqueteEditando = null;
        private DateTime? FechaEntregaEdit = null;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var usuario = await Http.GetFromJsonAsync<UsuarioActualResponse>("/api/usuarios/me");
                UsuarioEsAdmin = usuario?.RolNombre == "Admin" || usuario?.RolNombre == "SuperAdmin";
            }
            catch { UsuarioEsAdmin = false; }

            Empleados = await Http.GetFromJsonAsync<List<EmpleadoModel>>("/api/empleados?rol=chofer");
            await CargarVentas();
            await CargarPaquetesAsignados();
        }

        private async Task CargarVentas()
        {
            var rol = SessionState.Rol?.ToString()?.ToLower() ?? "";
            var usuarioId = SessionState.IdUsuario;
            var ventas = await Http.GetFromJsonAsync<List<VentaApiModel>>($"/api/envios/ventas-no-asignadas?usuarioId={usuarioId}&rol={rol}");
            VentasDisponibles = ventas?.Select(v => new VentaModel
            {
                Id = v.Id,
                Folio = v.Folio ?? $"VENTA-{v.Id}",
                DestinatarioNombre = v.Destinatario?.Nombre ?? ""
            }).ToList() ?? new();
        }

        private async Task CargarPaquetesAsignados()
        {
            // Cambia para consumir el endpoint filtrado por rol y usuario
            var rol = SessionState.Rol?.ToString()?.ToLower() ?? "";
            var usuarioId = SessionState.IdUsuario;
            var envios = await Http.GetFromJsonAsync<List<EnvioApiModel>>($"/api/envios/asignados?usuarioId={usuarioId}&rol={rol}");
            PaquetesAsignados = envios.Select(e => new PaqueteAsignadoModel
            {
                Id = e.Id,
                CodigoSeguimiento = e.GuiaRastreo ?? $"ENVIO-{e.Id}",
                FechaEntrega = e.FechaEntrega,
                EmpleadoAsignado = ObtenerNombreEmpleado(e.EmpleadoId)
            }).ToList();

            await CargarEstadosActualesAsync();
            StateHasChanged();
        }

        private async Task CargarEstadosActualesAsync()
        {
            var tasks = PaquetesAsignados.Select(async p =>
            {
                try
                {
                    var estado = await Http.GetFromJsonAsync<EstadoResp>($"/api/envios/{p.Id}/estado-actual");
                    if (estado != null)
                    {
                        p.Estado = estado.estado;
                        p.Entregado = estado.entregado;
                        p.FechaEstado = estado.fechaStatus;
                    }
                }
                catch { }
            }).ToList();
            await Task.WhenAll(tasks);
        }

        private string ObtenerNombreEmpleado(int? empleadoId)
        {
            if (!empleadoId.HasValue) return "-";
            var empleado = Empleados.FirstOrDefault(x => x.Id == empleadoId.Value);
            return empleado?.NombreCompleto ?? "-";
        }

        private async Task AsignarPaquete()
        {
            if (string.IsNullOrWhiteSpace(GuiaRastreo) || string.IsNullOrWhiteSpace(EmpleadoSeleccionadoId) || string.IsNullOrWhiteSpace(VentaSeleccionadaId))
                return;

            var request = new AsignarEnvioRequest
            {
                CodigoSeguimiento = GuiaRastreo,
                EmpleadoId = int.Parse(EmpleadoSeleccionadoId),
                FechaEntrega = FechaEntregaAsignar,
                VentaId = int.Parse(VentaSeleccionadaId)
            };
            var response = await Http.PostAsJsonAsync("/api/envios/asignar", request);
            if (response.IsSuccessStatusCode)
            {
                await CargarPaquetesAsignados();
                await CargarVentas(); // Refresca la lista de ventas disponibles al instante
                LimpiarFormulario();
                StateHasChanged(); // Fuerza el refresco visual inmediato
            }
        }

        private void LimpiarFormulario()
        {
            GuiaRastreo = string.Empty;
            EmpleadoSeleccionadoId = string.Empty;
            VentaSeleccionadaId = string.Empty;
            FechaEntregaAsignar = null;
        }

        // Modal fecha entrega
        private async void AbrirModalFecha(PaqueteAsignadoModel paquete)
        {
            // Consulta si puede editar fecha (no si Entregado o Incidencia Cerrada)
            try
            {
                var check = await Http.GetFromJsonAsync<PuedeEditarResp>($"/api/envios/{paquete.Id}/puede-editar-fecha");
                if (check != null && !check.puedeEditar)
                {
                    await MostrarAvisoAsync("No se puede editar la fecha de entrega: el envío ya fue entregado o tiene incidencia cerrada.");
                    return;
                }
            }
            catch { /* si falla, intentamos abrir igual */ }

            PaqueteEditando = paquete;
            FechaEntregaEdit = paquete.FechaEntrega;
            ModalFechaVisible = true;
            StateHasChanged();
        }
        private void CerrarModalFecha()
        {
            ModalFechaVisible = false;
            PaqueteEditando = null;
            FechaEntregaEdit = null;
        }
        private async Task GuardarFechaEntrega()
        {
            if (PaqueteEditando == null || FechaEntregaEdit == null) return;
            var response = await Http.PutAsJsonAsync($"/api/envios/{PaqueteEditando.Id}/fecha-entrega", new { fechaEntrega = FechaEntregaEdit });
            if (response.IsSuccessStatusCode)
            {
                PaqueteEditando.FechaEntrega = FechaEntregaEdit;
                ModalFechaVisible = false;
                await CargarPaquetesAsignados();
            }
            else
            {
                var msg = await response.Content.ReadAsStringAsync();
                await MostrarAvisoAsync(string.IsNullOrWhiteSpace(msg) ? "No se pudo actualizar la fecha de entrega." : msg);
            }
        }

        private Task MostrarAvisoAsync(string mensaje)
        {
            // Simple alert cross-platform en MAUI Blazor
            return JS.InvokeVoidAsync("alert", mensaje).AsTask();
        }

        private string GetEstadoCss(string? estado)
        {
            if (string.IsNullOrWhiteSpace(estado)) return string.Empty;
            var s = estado.Trim().ToLowerInvariant();
            return s switch
            {
                "entregado" => "entregado",
                "incidencia cerrada" => "incidencia-cerrada",
                "incidencia" => "incidencia",
                "en camino" => "en-camino",
                "último tramo" => "ultimo-tramo",
                "ultimo tramo" => "ultimo-tramo",
                "asignado" => "asignado",
                _ => "otro"
            };
        }

        public class PaqueteAsignadoModel
        {
            public int Id { get; set; }
            public string CodigoSeguimiento { get; set; } = string.Empty;
            public DateTime? FechaEntrega { get; set; }
            public string? EmpleadoAsignado { get; set; }
            // Nuevos campos de estado
            public string? Estado { get; set; }
            public bool Entregado { get; set; }
            public DateTime? FechaEstado { get; set; }
        }

        public class EmpleadoModel
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Apellido { get; set; } = string.Empty;
            public string NombreCompleto { get; set; } = string.Empty;
        }
        public class VentaModel
        {
            public int Id { get; set; }
            public string Folio { get; set; } = string.Empty;
            public string DestinatarioNombre { get; set; } = string.Empty;
        }
        public class EnvioApiModel
        {
            public int Id { get; set; }
            public string? GuiaRastreo { get; set; }
            public DateTime? FechaEntrega { get; set; }
            public int? EmpleadoId { get; set; }
            public string? EmpleadoAsignado { get; set; } // Cambia a EmpleadoAsignado para reflejar el backend
        }
        private class PuedeEditarResp { public int envioId { get; set; } public bool puedeEditar { get; set; } }
        private class EstadoResp { public int envioId { get; set; } public string estado { get; set; } = string.Empty; public DateTime? fechaStatus { get; set; } public bool entregado { get; set; } }
        public class VentaApiModel
        {
            public int Id { get; set; }
            public string? Folio { get; set; }
            public DestinatarioApiModel? Destinatario { get; set; }
            public string? DestinatarioNombre { get; set; } // Para compatibilidad con el endpoint
        }
        public class DestinatarioApiModel
        {
            public string? Nombre { get; set; }
        }
        public class AsignarEnvioRequest
        {
            public string CodigoSeguimiento { get; set; } = string.Empty;
            public int EmpleadoId { get; set; }
            public DateTime? FechaEntrega { get; set; }
            public int VentaId { get; set; }
        }
        public class UsuarioActualResponse
        {
            [JsonPropertyName("RolNombre")]
            public string RolNombre { get; set; }
        }
    }
}
