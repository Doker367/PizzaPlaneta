using ManyBox.Models.Api;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManyBox.Services
{
    public class ClienteApiService
    {
        private readonly HttpClient _httpClient;

        public ClienteApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ManyBox.Models.Api.Cliente>> GetClientesAsync(string? nombre = null, string? correo = null)
        {
            var url = "api/clientes";
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(nombre))
            {
                queryParams.Add($"nombre={nombre}");
            }
            if (!string.IsNullOrEmpty(correo))
            {
                queryParams.Add($"correo={correo}");
            }
            if (queryParams.Count > 0)
            {
                url += "?" + string.Join("&", queryParams);
            }

            return await _httpClient.GetFromJsonAsync<List<ManyBox.Models.Api.Cliente>>(url);
        }

        public async Task<ManyBox.Models.Api.Cliente> GetClienteByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ManyBox.Models.Api.Cliente>($"api/clientes/{id}");
        }

        public async Task<ManyBox.Models.Api.Cliente> CrearClienteAsync(CrearClienteRequest cliente)
        {
            var response = await _httpClient.PostAsJsonAsync("api/clientes", cliente);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ManyBox.Models.Api.Cliente>();
        }
    }
}
