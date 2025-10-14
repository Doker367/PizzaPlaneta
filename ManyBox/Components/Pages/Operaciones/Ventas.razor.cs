using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ManyBox.Utils;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ManyBox.Components.Pages.Operaciones
{
    public partial class Ventas
    {
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private HttpClient Http { get; set; } = default!;

        private List<VentaModel>? ventas;
        private VentaModel? ventaDetalle;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var rol = SessionState.Rol?.ToString()?.ToLower() ?? "";
                var usuarioId = SessionState.IdUsuario;
                // Cambiamos al nuevo endpoint para que 'completada' venga 100% de la tabla ventas
                var url = $"/api/ventas-estado/filtrados?usuarioId={usuarioId}&rol={rol}";

                var opts = new JsonSerializerOptions(JsonSerializerDefaults.Web);
                opts.Converters.Add(new BoolFlexibleJsonConverter());

                var response = await Http.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    ventas = JsonSerializer.Deserialize<List<VentaModel>>(json, opts) ?? new List<VentaModel>();
                }
                else
                {
                    ventas = new List<VentaModel>();
                }

                await ApplyVentasStylesAsync();
            }
            catch
            {
                ventas = new List<VentaModel>();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await ApplyVentasStylesAsync();
            }

            if (ventaDetalle != null)
            {
                await JSRuntime.InvokeVoidAsync("eval", "if (typeof setupModalScroll === 'function') { setupModalScroll(); }");
            }
        }

        private async Task ApplyVentasStylesAsync()
        {
            await JSRuntime.InvokeVoidAsync("applyVentasStyles");
        }

        void MostrarDetalle(VentaModel venta)
        {
            ventaDetalle = venta;
        }

        void CerrarDetalle()
        {
            ventaDetalle = null;
        }

        void TerminarDesdeHistorial(int ventaId)
        {
            NavigationManager.NavigateTo($"/registro-venta/{ventaId}");
        }

        public class VentaModel
        {
            [JsonPropertyName("id")] public int Id { get; set; }
            [JsonPropertyName("fecha")] public DateTime Fecha { get; set; }
            [JsonPropertyName("folio")] public required string Folio { get; set; }
            [JsonPropertyName("compania_Envio")] public string? Compania_Envio { get; set; }
            [JsonPropertyName("total_Cobrado")] public decimal? Total_Cobrado { get; set; }
            [JsonPropertyName("tipo_Pago")] public string? Tipo_Pago { get; set; }
            [JsonPropertyName("seguro")] public bool? Seguro { get; set; }
            [JsonPropertyName("total_Piezas")] public int? Total_Piezas { get; set; }
            [JsonPropertyName("tiempo_Estimado")] public string? Tiempo_Estimado { get; set; }
            [JsonPropertyName("sucursalOrigen")] public string? SucursalOrigen { get; set; }
            [JsonPropertyName("remitente")] public RemitenteModel? Remitente { get; set; }
            [JsonPropertyName("destinatario")] public DestinatarioModel? Destinatario { get; set; }
            [JsonPropertyName("detalleContenido")] public List<DetalleContenidoModel>? DetalleContenido { get; set; }
            public required string DestinatarioNombre { get; set; }
            [JsonPropertyName("completada")] public bool? Completada { get; set; }

            public class RemitenteModel
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
            public class DestinatarioModel
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
            public class DetalleContenidoModel
            {
                public string? Descripcion { get; set; }
                public decimal Cantidad { get; set; }
                public string? Unidad { get; set; }
            }
        }
    }
}