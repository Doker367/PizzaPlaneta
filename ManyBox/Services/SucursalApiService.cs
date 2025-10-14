using ManyBox.Models.Api;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ManyBox.Services
{
    public class SucursalApiService
    {
        private readonly HttpClient _httpClient;

        public SucursalApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Sucursal>> GetSucursalesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Sucursal>>("api/sucursales");
        }

        public async Task CrearSucursalAsync(Sucursal sucursal)
        {
            var response = await _httpClient.PostAsJsonAsync("api/sucursales", sucursal);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<Sucursal>> GetSucursalesAsync(string? nombre = null)
        {
            string url = "api/sucursales";
            if (!string.IsNullOrWhiteSpace(nombre))
                url = $"api/sucursales/filtrar?nombre={nombre}";
            return await _httpClient.GetFromJsonAsync<List<Sucursal>>(url) ?? new List<Sucursal>();
        }

        public async Task<HttpResponseMessage> RegistrarSucursalAsync(Sucursal sucursal)
        {
            return await _httpClient.PostAsJsonAsync("api/sucursales", sucursal);
        }

        public async Task<HttpResponseMessage> EliminarSucursalAsync(int sucursalId)
        {
            return await _httpClient.DeleteAsync($"api/sucursales/{sucursalId}");
        }
    }
}
