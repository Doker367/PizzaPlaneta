using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ManyBox.Utils;

namespace ManyBox.Components.Pages.Admin
{
    public partial class Paquetes
    {
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private List<PaqueteModel> paquetes = new();
        private List<PaqueteModel> paquetesFiltrados = new();

        private string filtroDestinatario = "";
        private DateTime? filtroFecha = null;
        private string filtroEstado = "";

        private bool UsuarioEsAdmin = true;
        private bool mostrarDetalles = false;
        private PaqueteModel? paqueteSeleccionado;

        private string? guiaRastreoDetalle = null;
        private bool cargandoGuia = false;

        private List<string> EstadosPosibles = new()
        {
            "Pendiente", "En tránsito", "Entregado", "Devuelto", "Extraviado", "Registrado", "Asignado"
        };

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var rol = SessionState.Rol?.ToString()?.ToLower() ?? "";
                var usuarioId = SessionState.IdUsuario;

                // Intenta primero el endpoint nuevo; si no existe (404), cae al antiguo
                var filtradosUrl = $"/api/paquetes/filtrados?usuarioId={usuarioId}&rol={rol}";
                List<PaqueteApiModel>? paquetesApi = null;

                try
                {
                    var resp = await Http.GetAsync(filtradosUrl);
                    if (resp.IsSuccessStatusCode)
                    {
                        paquetesApi = await resp.Content.ReadFromJsonAsync<List<PaqueteApiModel>>();
                    }
                    else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // Fallback
                        var resp2 = await Http.GetAsync("/api/paquetes");
                        if (resp2.IsSuccessStatusCode)
                        {
                            paquetesApi = await resp2.Content.ReadFromJsonAsync<List<PaqueteApiModel>>();
                        }
                        else
                        {
                            Console.WriteLine($"Backend respondió {resp2.StatusCode} para /api/paquetes");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Backend respondió {resp.StatusCode} para /api/paquetes/filtrados");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error consultando paquetes: {ex.Message}");
                }

                if (paquetesApi == null)
                {
                    paquetes = new();
                    paquetesFiltrados = new();
                    return;
                }

                paquetes = paquetesApi.Select(p => new PaqueteModel
                {
                    Id = p.Id,
                    CodigoSeguimiento = p.CodigoSeguimiento,
                    FechaRegistro = p.FechaRegistro,
                    SucursalOrigen = p.SucursalOrigen ?? "-",
                    RemitenteNombre = p.RemitenteNombre ?? "",
                    RemitenteTelefono = p.RemitenteTelefono ?? "",
                    DestinatarioNombre = p.DestinatarioNombre ?? "",
                    DestinatarioTelefono = p.DestinatarioTelefono ?? "",
                    DireccionDestino = p.DireccionDestino ?? "",
                    CiudadDestino = p.CiudadDestino ?? "",
                    TipoEnvio = p.TipoEnvio ?? "",
                    EstadoActual = p.EstadoActual ?? "Registrado",
                    FechaUltimaActualizacion = p.FechaUltimaActualizacion,
                    EmpleadoRegistro = p.EmpleadoRegistro ?? "-",
                    EmpleadoActual = p.EmpleadoActual ?? "-",
                    RepartidorAsignado = p.RepartidorAsignado ?? "-",
                    PesoKg = p.PesoKg,
                    NumeroGuia = p.NumeroGuia ?? p.CodigoSeguimiento,
                    PaquetesAsignados = p.PaquetesAsignados ?? "-",
                    CostoEnvio = p.CostoEnvio,
                    VentaId = p.VentaId // <--- Asegura que se mapea correctamente
                }).ToList();

                paquetesFiltrados = paquetes.ToList();
            }
            catch (Exception ex)
            {
                paquetes = new();
                paquetesFiltrados = new();
                Console.WriteLine($"Error al cargar paquetes: {ex.Message}");
            }
        }

        private void FiltrarPaquetes()
        {
            paquetesFiltrados = paquetes.Where(p =>
                (string.IsNullOrWhiteSpace(filtroDestinatario) || p.DestinatarioNombre.Contains(filtroDestinatario, StringComparison.OrdinalIgnoreCase)) &&
                (!filtroFecha.HasValue || p.FechaRegistro.Date == filtroFecha.Value.Date) &&
                (string.IsNullOrWhiteSpace(filtroEstado) || p.EstadoActual == filtroEstado)
            ).ToList();
        }

        private async Task VerDetallesAsync(int id)
        {
            paqueteSeleccionado = paquetes.FirstOrDefault(p => p.Id == id);
            if (paqueteSeleccionado != null)
            {
                mostrarDetalles = true;
            }
        }

        private void VerDetalles(int id) => _ = VerDetallesAsync(id);

        private async Task DescargarComprobanteAsync(int id)
        {
            try
            {
                var response = await Http.GetAsync($"/api/paquetes/{id}/nota-venta-pdf");
                if (response.IsSuccessStatusCode)
                {
                    var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                    var fileName = $"NotaVenta_{id}.pdf";
                    using var stream = new MemoryStream(pdfBytes);
                    using var streamRef = new DotNetStreamReference(stream);
                    await JSRuntime.InvokeVoidAsync("descargarArchivo", fileName, streamRef);
                }
                else
                {
                    Console.WriteLine($"Error al descargar el PDF: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al descargar el PDF: {ex.Message}");
            }
        }

        private void DescargarComprobante(int id) => _ = DescargarComprobanteAsync(id);

        private void Editar(int id) { NavigationManager.NavigateTo($"/paquetes/editar/{id}"); }
        private async Task EliminarAsync(int id)
        {
            var response = await Http.DeleteAsync($"/api/ventas/{id}");
            if (response.IsSuccessStatusCode)
            {
                paquetes.RemoveAll(p => p.Id == id);
                FiltrarPaquetes();
            }
            else
            {
                Console.WriteLine($"Error al eliminar el paquete: {response.StatusCode}");
            }
        }

        private void Eliminar(int id) => _ = EliminarAsync(id);

        private void PrefillDhlRates(PaqueteModel p)
        {
            if (string.IsNullOrWhiteSpace(p?.TipoEnvio) || !p.TipoEnvio.Contains("DHL", StringComparison.OrdinalIgnoreCase))
                return;

            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(p.CiudadDestino))
                qs.Add($"destinationCityName={Uri.EscapeDataString(p.CiudadDestino)}");

            qs.Add("unitOfMeasurement=metric");
            qs.Add($"plannedShippingDate={DateTime.Today:yyyy-MM-dd}");

            if (p.PesoKg > 0)
                qs.Add($"weight={p.PesoKg.ToString(System.Globalization.CultureInfo.InvariantCulture)}");

            var url = "/dhl/rates" + (qs.Count > 0 ? ("?" + string.Join("&", qs)) : string.Empty);
            NavigationManager.NavigateTo(url);
        }

        private void PrefillDhlCreate(PaqueteModel p)
        {
            if (string.IsNullOrWhiteSpace(p?.TipoEnvio) || !p.TipoEnvio.Contains("DHL", StringComparison.OrdinalIgnoreCase))
                return;

            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(p.DestinatarioNombre))
                qs.Add($"receiverFullName={Uri.EscapeDataString(p.DestinatarioNombre)}");
            if (!string.IsNullOrWhiteSpace(p.DestinatarioTelefono))
                qs.Add($"receiverPhone={Uri.EscapeDataString(p.DestinatarioTelefono)}");
            if (!string.IsNullOrWhiteSpace(p.DireccionDestino))
                qs.Add($"receiverAddressLine1={Uri.EscapeDataString(p.DireccionDestino)}");
            if (!string.IsNullOrWhiteSpace(p.CiudadDestino))
                qs.Add($"receiverCity={Uri.EscapeDataString(p.CiudadDestino)}");

            if (p.PesoKg > 0)
                qs.Add($"packageWeight={p.PesoKg.ToString(System.Globalization.CultureInfo.InvariantCulture)}");

            qs.Add($"plannedDate={DateTime.Today:yyyy-MM-dd}");
            qs.Add("unit=metric");

            var url = "/dhl/create-shipment" + (qs.Count > 0 ? ("?" + string.Join("&", qs)) : string.Empty);
            NavigationManager.NavigateTo(url);
        }

        public class PaqueteModel
        {
            public int Id { get; set; }
            public required string CodigoSeguimiento { get; set; }
            public DateTime FechaRegistro { get; set; }
            public string? SucursalOrigen { get; set; }
            public required string RemitenteNombre { get; set; }
            public required string RemitenteTelefono { get; set; }
            public required string DestinatarioNombre { get; set; }
            public required string DestinatarioTelefono { get; set; }
            public required string DireccionDestino { get; set; }
            public required string CiudadDestino { get; set; }
            public required string TipoEnvio { get; set; }
            public required string EstadoActual { get; set; }
            public DateTime? FechaUltimaActualizacion { get; set; }
            public string? EmpleadoRegistro { get; set; }
            public required string EmpleadoActual { get; set; }
            public required string RepartidorAsignado { get; set; }
            public decimal PesoKg { get; set; }
            public required string NumeroGuia { get; set; }
            public required string PaquetesAsignados { get; set; }
            public decimal CostoEnvio { get; set; }
            public int VentaId { get; set; } // Agregado para vincular con la venta
        }

        public class PaqueteApiModel
        {
            public int Id { get; set; }
            public string CodigoSeguimiento { get; set; }
            public DateTime FechaRegistro { get; set; }
            public string? SucursalOrigen { get; set; }
            public string? RemitenteNombre { get; set; }
            public string? RemitenteTelefono { get; set; }
            public string? DestinatarioNombre { get; set; }
            public string? DestinatarioTelefono { get; set; }
            public string? DireccionDestino { get; set; }
            public string? CiudadDestino { get; set; }
            public string? TipoEnvio { get; set; }
            public string? EstadoActual { get; set; }
            public DateTime? FechaUltimaActualizacion { get; set; }
            public string? EmpleadoRegistro { get; set; }
            public string? EmpleadoActual { get; set; }
            public string? RepartidorAsignado { get; set; }
            public decimal PesoKg { get; set; }
            public string? NumeroGuia { get; set; }
            public string? PaquetesAsignados { get; set; }
            public decimal CostoEnvio { get; set; }
            public int VentaId { get; set; } // <-- Agregado para que el mapeo funcione
        }

        public class EnvioDetalleDto
        {
            public string? GuiaRastreo { get; set; }
        }
    }
}
