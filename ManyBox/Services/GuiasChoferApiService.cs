using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using ManyBox.Models;

namespace ManyBox.Services
{
    public class GuiasChoferApiService
    {
        private readonly HttpClient _httpClient;
        public GuiasChoferApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<GuiaChoferDto>> ObtenerGuiasPorChoferAsync(int empleadoId)
        {
            // CORREGIDO: endpoint correcto según backend
            var result = await _httpClient.GetFromJsonAsync<List<GuiaChoferDto>>($"api/envios/asignados/empleado/{empleadoId}");
            return result ?? new List<GuiaChoferDto>();
        }

        public async Task<bool> CambiarEstadoGuiaAsync(int envioId, string status)
        {
            var req = new { status };
            var response = await _httpClient.PostAsync($"api/envios/{envioId}/cambiar-estado",
                new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }
    }
}
