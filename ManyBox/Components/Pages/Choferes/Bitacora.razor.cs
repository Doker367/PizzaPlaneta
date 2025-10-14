using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Globalization;
using ManyBox.Models.Client; // BitacoraEntregaDto

namespace ManyBox.Components.Pages.Choferes
{
    public partial class Bitacora
    {
        private List<EntregaBitacora> entregas = new();
        private int puntualidadPorcentaje = 0;
        private int eficienciaPorcentaje = 0;
        private bool isLoading = true;
        private string? errorMsg;

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            errorMsg = null;
            try
            {
                int empleadoId = ManyBox.Utils.SessionState.EmpleadoId;
                if (empleadoId == 0 && Microsoft.Maui.Storage.Preferences.Default.ContainsKey("empleadoId"))
                {
                    empleadoId = Microsoft.Maui.Storage.Preferences.Default.Get("empleadoId", 0);
                    ManyBox.Utils.SessionState.EmpleadoId = empleadoId;
                }
                if (empleadoId == 0)
                {
                    errorMsg = "No se encontró el empleado logueado. Por favor, vuelve a iniciar sesión o contacta a soporte.";
                    isLoading = false;
                    return;
                }
                var http = new HttpClient { BaseAddress = new Uri(Microsoft.Maui.Storage.Preferences.Default.Get("apiBaseUrl", "http://100.64.197.11:5000/")) };
                var result = await http.GetFromJsonAsync<List<EntregaBitacora>>($"api/envios/bitacora/{empleadoId}");
                entregas = result ?? new List<EntregaBitacora>();
                CalcularEstadisticas();
            }
            catch (Exception ex)
            {
                errorMsg = $"Error al cargar la bitácora: {ex.Message}";
                entregas = new();
            }
            isLoading = false;
        }

        private void CalcularEstadisticas()
        {
            if (entregas.Count == 0)
            {
                puntualidadPorcentaje = 0;
                eficienciaPorcentaje = 0;
                return;
            }
            int totalEntregas = entregas.Count;
            int entregasATiempo = entregas.Count(e => e.Estado.Equals("Entregado", StringComparison.OrdinalIgnoreCase));
            puntualidadPorcentaje = (int)Math.Round((double)entregasATiempo / totalEntregas * 100);
            eficienciaPorcentaje = puntualidadPorcentaje;
        }

        private class EntregaBitacora
        {
            public string Guia { get; set; } = string.Empty;
            public string Direccion { get; set; } = string.Empty;
            public string Destinatario { get; set; } = string.Empty;
            public DateTime Fecha { get; set; }
            public string Estado { get; set; } = string.Empty;
        }
    }
}