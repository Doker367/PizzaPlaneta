using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using ManyBox.Models.Client; // sustituye ManyBoxApi.DTOs
using ManyBox.Utils;
using System.Linq;

namespace ManyBox.Components.Pages.Operaciones
{
    public partial class RegistroVenta
    {
        [Parameter] public int? Id { get; set; }

        private CrearVentaRequest venta = new()
        {
            DetalleContenido = new List<DetalleContenidoRequest> { new DetalleContenidoRequest() },
            Fecha = DateTime.Now,
            Completada = true
        };
        private string mensaje = string.Empty;
        private string busquedaCliente = string.Empty;
        private List<ClienteModel> clientesEncontrados = new();
        private ClienteModel? clienteSeleccionado;

        protected override async Task OnInitializedAsync()
        {
            // Asignación automática del empleado actual
            venta.Empleado_Id = SessionState.IdUsuario;

            // Si se abre para editar/terminar, traer la venta y no regenerar folio
            if (Id.HasValue && Id.Value > 0)
            {
                try
                {
                    var v = await Http.GetFromJsonAsync<VentaDetalleApi>($"/api/ventas/{Id.Value}");
                    if (v != null)
                    {
                        venta = new CrearVentaRequest
                        {
                            Fecha = v.Fecha,
                            Cliente_Id = v.ClienteId,
                            Empleado_Id = SessionState.IdUsuario,
                            // Mantener el folio existente del backend
                            Folio = string.IsNullOrWhiteSpace(v.Folio) ? null : v.Folio,
                            Remitente_Nombre = v.Remitente?.Nombre ?? string.Empty,
                            Remitente_Telefono = v.Remitente?.Telefono ?? string.Empty,
                            Remitente_Compania = v.Remitente?.Compania,
                            Remitente_Direccion = v.Remitente?.Direccion ?? string.Empty,
                            Remitente_Ciudad = v.Remitente?.Ciudad ?? string.Empty,
                            Remitente_Estado = v.Remitente?.Estado ?? string.Empty,
                            Remitente_Pais = v.Remitente?.Pais ?? string.Empty,
                            Remitente_Cp = v.Remitente?.CP ?? string.Empty,
                            Destinatario_Nombre = v.Destinatario?.Nombre ?? string.Empty,
                            Destinatario_Telefono = v.Destinatario?.Telefono ?? string.Empty,
                            Destinatario_Compania = v.Destinatario?.Compania,
                            Destinatario_Direccion = v.Destinatario?.Direccion ?? string.Empty,
                            Destinatario_Ciudad = v.Destinatario?.Ciudad ?? string.Empty,
                            Destinatario_Estado = v.Destinatario?.Estado ?? string.Empty,
                            Destinatario_Pais = v.Destinatario?.Pais ?? string.Empty,
                            Destinatario_Cp = v.Destinatario?.CP ?? string.Empty,
                            Valor_Declarado = v.Valor_Declarado,
                            Medidas = v.Medidas ?? string.Empty,
                            Peso_Volumetrico = v.Peso_Volumetrico,
                            Peso_Fisico = v.Peso_Fisico,
                            Seguro = v.Seguro,
                            Compania_Envio = v.Compania_Envio ?? string.Empty,
                            Tipo_Riesgo = v.Tipo_Riesgo ?? string.Empty,
                            Tipo_Pago = v.Tipo_Pago ?? string.Empty,
                            Costo_Envio = v.Costo_Envio,
                            Total_Piezas = v.Total_Piezas,
                            Tiempo_Estimado = v.Tiempo_Estimado ?? string.Empty,
                            Total_Cobrado = v.Total_Cobrado,
                            DetalleContenido = v.DetalleContenido?.Select(d => new DetalleContenidoRequest
                            {
                                Descripcion = d.Descripcion ?? string.Empty,
                                Cantidad = d.Cantidad,
                                Unidad = d.Unidad
                            }).ToList() ?? new List<DetalleContenidoRequest> { new DetalleContenidoRequest() },
                            Completada = v.Completada
                        };

                        // Autoseleccionar cliente en la UI
                        if (v.ClienteId.HasValue)
                        {
                            clienteSeleccionado = new ClienteModel
                            {
                                Id = v.ClienteId.Value,
                                Nombre = v.ClienteNombre ?? string.Empty,
                                Apellido = string.Empty,
                                Correo = string.Empty,
                                Telefono = string.Empty
                            };
                        }
                    }
                }
                catch { }
            }
            else
            {
                // Nuevo: generar solo un folio provisional para mostrar (el backend puede respetarlo si se envía)
                var sucId = await GetSucursalIdActualAsync();
                venta.Folio = GenerarFolio(sucId, 0);
            }
        }

        private async Task<int?> GetSucursalIdActualAsync()
        {
            try
            {
                var userId = SessionState.IdUsuario;
                return await Http.GetFromJsonAsync<int?>($"/api/usuarios/{userId}/sucursal-id");
            }
            catch { return null; }
        }

        // Formato requerido: S + sucursal(2 dígitos, 0-izq) + yyyyMMdd + ventaId
        private static string GenerarFolio(int? sucursalId, int ventaId)
        {
            var sucStr = (sucursalId ?? 0) < 10 ? $"0{(sucursalId ?? 0)}" : (sucursalId ?? 0).ToString();
            var idStr = ventaId.ToString("D2");
            return $"S{sucStr}{DateTime.Now:yyyyMMdd}{idStr}";
        }

        private async Task BuscarCliente()
        {
            if (string.IsNullOrWhiteSpace(busquedaCliente) || busquedaCliente.Length < 2)
            {
                clientesEncontrados = new();
                return;
            }
            var allClientes = await Http.GetFromJsonAsync<List<ClienteModel>>($"api/clientes?search={Uri.EscapeDataString(busquedaCliente)}") ?? new();
            clientesEncontrados = allClientes
                .Where(c =>
                    (!string.IsNullOrEmpty(c.Nombre) && c.Nombre.Contains(busquedaCliente, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Apellido) && c.Apellido.Contains(busquedaCliente, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Correo) && c.Correo.Contains(busquedaCliente, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Telefono) && c.Telefono.Contains(busquedaCliente, StringComparison.OrdinalIgnoreCase))
                )
                .Take(5)
                .ToList();
        }

        private void SeleccionarCliente(ClienteModel cliente)
        {
            clienteSeleccionado = cliente;
            venta.Cliente_Id = cliente.Id;
        }

        private void AgregarDetalleContenido()
        {
            venta.DetalleContenido.Add(new DetalleContenidoRequest());
        }

        private async Task RegistrarVenta()
        {
            venta.Empleado_Id = SessionState.IdUsuario;
            mensaje = string.Empty;

            if (venta.Cliente_Id == null || venta.Cliente_Id == 0)
            {
                mensaje = "• Debes seleccionar un cliente.";
                return;
            }
            if (string.IsNullOrWhiteSpace(venta.Folio) ||
                string.IsNullOrWhiteSpace(venta.Remitente_Nombre) ||
                string.IsNullOrWhiteSpace(venta.Remitente_Telefono) ||
                string.IsNullOrWhiteSpace(venta.Remitente_Direccion) ||
                string.IsNullOrWhiteSpace(venta.Remitente_Ciudad) ||
                string.IsNullOrWhiteSpace(venta.Remitente_Estado) ||
                string.IsNullOrWhiteSpace(venta.Remitente_Pais) ||
                string.IsNullOrWhiteSpace(venta.Remitente_Cp) ||
                string.IsNullOrWhiteSpace(venta.Destinatario_Nombre) ||
                string.IsNullOrWhiteSpace(venta.Destinatario_Telefono) ||
                string.IsNullOrWhiteSpace(venta.Destinatario_Direccion) ||
                string.IsNullOrWhiteSpace(venta.Destinatario_Ciudad) ||
                string.IsNullOrWhiteSpace(venta.Destinatario_Estado) ||
                string.IsNullOrWhiteSpace(venta.Destinatario_Pais) ||
                string.IsNullOrWhiteSpace(venta.Destinatario_Cp) ||
                venta.Total_Piezas == null || venta.Total_Piezas == 0 ||
                venta.Total_Cobrado == null || venta.Total_Cobrado == 0 ||
                venta.Costo_Envio == null || venta.Costo_Envio == 0 ||
                venta.Peso_Fisico == null || venta.Peso_Fisico == 0 ||
                venta.Peso_Volumetrico == null || venta.Peso_Volumetrico == 0 ||
                string.IsNullOrWhiteSpace(venta.Compania_Envio) ||
                string.IsNullOrWhiteSpace(venta.Tipo_Pago) ||
                string.IsNullOrWhiteSpace(venta.Tiempo_Estimado) ||
                string.IsNullOrWhiteSpace(venta.Medidas) ||
                venta.Valor_Declarado == null || venta.Valor_Declarado == 0 ||
                string.IsNullOrWhiteSpace(venta.Tipo_Riesgo))
            {
                mensaje = "• Todos los campos obligatorios deben estar completos.";
                return;
            }
            if (venta.DetalleContenido == null || !venta.DetalleContenido.Any(x => !string.IsNullOrWhiteSpace(x.Descripcion) && x.Cantidad > 0 && !string.IsNullOrWhiteSpace(x.Unidad)))
            {
                mensaje = "• Debes agregar al menos un detalle de contenido válido.";
                return;
            }

            try
            {
                venta.Completada = true;
                if (string.IsNullOrWhiteSpace(venta.Folio))
                {
                    var sucId = await GetSucursalIdActualAsync();
                    venta.Folio = GenerarFolio(sucId, Id ?? 0);
                }

                if (Id.HasValue && Id.Value > 0)
                {
                    var responsePut = await Http.PutAsJsonAsync($"/api/ventas/{Id.Value}", venta);
                    if (responsePut.IsSuccessStatusCode)
                    {
                        mensaje = "? Venta actualizada correctamente.";
                    }
                    else
                    {
                        var errorMsgPut = await responsePut.Content.ReadAsStringAsync();
                        mensaje = $"Error al actualizar la venta. {errorMsgPut}";
                    }
                }
                else
                {
                    var response = await Http.PostAsJsonAsync("/api/ventas", venta);
                    if (response.IsSuccessStatusCode)
                    {
                        var created = await response.Content.ReadFromJsonAsync<CreatedVentaResp>();
                        if (created != null && created.Id > 0)
                        {
                            var sucId = await GetSucursalIdActualAsync();
                            var folioFinal = GenerarFolio(sucId, created.Id);
                            venta.Folio = folioFinal;
                            await Http.PutAsJsonAsync($"/api/ventas/{created.Id}", venta);
                        }

                        mensaje = "? Venta registrada correctamente.";
                        venta = new CrearVentaRequest { DetalleContenido = new List<DetalleContenidoRequest> { new DetalleContenidoRequest() }, Fecha = DateTime.Now, Completada = true };
                        venta.Empleado_Id = SessionState.IdUsuario;
                        var sucId2 = await GetSucursalIdActualAsync();
                        venta.Folio = GenerarFolio(sucId2, 0);
                        clienteSeleccionado = null;
                    }
                    else
                    {
                        var errorMsg = await response.Content.ReadAsStringAsync();
                        mensaje = $"Error al registrar la venta. {errorMsg}";
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error: {ex.Message}";
            }
        }

        private async Task GuardarIncompleta()
        {
            venta.Empleado_Id = SessionState.IdUsuario;
            mensaje = string.Empty;

            if (venta.Cliente_Id == null || venta.Cliente_Id == 0)
            {
                mensaje = "Selecciona un cliente para guardar la venta.";
                return;
            }
            if (string.IsNullOrWhiteSpace(venta.Remitente_Nombre) || string.IsNullOrWhiteSpace(venta.Destinatario_Nombre))
            {
                mensaje = "Captura al menos Remitente y Destinatario.";
                return;
            }

            // Relleno para evitar validación 400 del backend
            venta.Folio = string.IsNullOrWhiteSpace(venta.Folio) ? GenerarFolio(await GetSucursalIdActualAsync(), Id ?? 0) : venta.Folio;
            venta.Tipo_Pago ??= "Pendiente";
            venta.Tipo_Riesgo ??= "Pendiente";
            venta.Compania_Envio ??= "Pendiente";
            venta.Tiempo_Estimado ??= "-";
            venta.Medidas ??= "-";
            venta.Remitente_Compania ??= null; // opcional
            venta.Destinatario_Compania ??= null; // opcional
            if (venta.DetalleContenido == null || venta.DetalleContenido.Count == 0)
                venta.DetalleContenido = new List<DetalleContenidoRequest> { new DetalleContenidoRequest { Descripcion = "-", Cantidad = 0, Unidad = "-" } };
            else
            {
                venta.DetalleContenido[0].Descripcion ??= "-";
                if (venta.DetalleContenido[0].Unidad == null) venta.DetalleContenido[0].Unidad = "-";
            }
            venta.Completada = false;

            try
            {
                if (Id.HasValue && Id.Value > 0)
                {
                    var responsePut = await Http.PutAsJsonAsync($"/api/ventas/{Id.Value}", venta);
                    if (responsePut.IsSuccessStatusCode)
                    {
                        mensaje = "Venta guardada como incompleta.";
                    }
                    else
                    {
                        var errorMsgPut = await responsePut.Content.ReadAsStringAsync();
                        mensaje = $"Error al guardar: {errorMsgPut}";
                    }
                }
                else
                {
                    var response = await Http.PostAsJsonAsync("/api/ventas", venta);
                    if (response.IsSuccessStatusCode)
                    {
                        var created = await response.Content.ReadFromJsonAsync<CreatedVentaResp>();
                        if (created != null && created.Id > 0)
                        {
                            var sucId = await GetSucursalIdActualAsync();
                            venta.Folio = GenerarFolio(sucId, created.Id);
                            await Http.PutAsJsonAsync($"/api/ventas/{created.Id}", venta);
                        }
                        mensaje = "Venta guardada como incompleta.";
                    }
                    else
                    {
                        var errorMsg = await response.Content.ReadAsStringAsync();
                        mensaje = $"Error al guardar: {errorMsg}";
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error: {ex.Message}";
            }
        }

        private bool seguroAux
        {
            get => venta.Seguro ?? false;
            set => venta.Seguro = value;
        }

        public class ClienteModel
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Apellido { get; set; } = string.Empty;
            public string Correo { get; set; } = string.Empty;
            public string Telefono { get; set; } = string.Empty;
        }

        private async Task OnBuscarClienteSubmit(EventArgs e)
        {
            await BuscarCliente();
        }

        private class CreatedVentaResp { public int Id { get; set; } }

        private class VentaDetalleApi
        {
            public int Id { get; set; }
            public DateTime Fecha { get; set; }
            public string? Folio { get; set; }
            public int? ClienteId { get; set; }
            public string? ClienteNombre { get; set; }
            public Persona? Remitente { get; set; }
            public Persona? Destinatario { get; set; }
            public decimal? Valor_Declarado { get; set; }
            public string? Medidas { get; set; }
            public decimal? Peso_Volumetrico { get; set; }
            public decimal? Peso_Fisico { get; set; }
            public bool? Seguro { get; set; }
            public string? Compania_Envio { get; set; }
            public string? Tipo_Riesgo { get; set; }
            public string? Tipo_Pago { get; set; }
            public decimal? Costo_Envio { get; set; }
            public int? Total_Piezas { get; set; }
            public string? Tiempo_Estimado { get; set; }
            public decimal? Total_Cobrado { get; set; }
            public List<DetalleContenidoItem>? DetalleContenido { get; set; }
            public bool Completada { get; set; }

            public class Persona
            {
                public string? Nombre { get; set; }
                public string? Telefono { get; set; }
                public string? Compania { get; set; }
                public string? Direccion { get; set; }
                public string? Ciudad { get; set; }
                public string? Estado { get; set; }
                public string? Pais { get; set; }
                public string? CP { get; set; }
            }

            public class DetalleContenidoItem
            {
                public string? Descripcion { get; set; }
                public decimal Cantidad { get; set; }
                public string? Unidad { get; set; }
            }
        }
    }
}
