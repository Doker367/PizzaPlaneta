
using ManyBox.Models.Api;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ManyBox.Services
{
    public class UsuarioApiService
    {
        private readonly HttpClient _httpClient;

        public UsuarioApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CrearUsuarioEmpleadoAsync(CreateUsuarioEmpleadoVM empleado)
        {
            var response = await _httpClient.PostAsJsonAsync("api/usuarios", empleado);
            response.EnsureSuccessStatusCode();
        }
    }
}
