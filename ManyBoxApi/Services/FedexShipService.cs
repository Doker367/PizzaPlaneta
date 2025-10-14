using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ManyBoxApi.Services
{
    public class FedexShipService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public FedexShipService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        private async Task<string> ObtenerTokenFedexAsync()
        {
            var clientId = _config["Fedex:ClientId"];
            var clientSecret = _config["Fedex:ClientSecret"];
            var cuerpo = $"grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}";

            var request = new HttpRequestMessage(HttpMethod.Post, "https://apis-sandbox.fedex.com/oauth/token")
            {
                Content = new StringContent(cuerpo, Encoding.UTF8, "application/x-www-form-urlencoded")
            };

            var respuesta = await _httpClient.SendAsync(request);
            var json = await respuesta.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("access_token").GetString() ?? string.Empty;
        }

        public async Task<(byte[] content, string contentType, string? filePath)> CreateShipmentAsync(object peticionFedex)
        {
            var token = await ObtenerTokenFedexAsync();
            var urlApi = "https://apis-sandbox.fedex.com/ship/v1/shipments";

            var request = new HttpRequestMessage(HttpMethod.Post, urlApi);
            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Headers.Add("X-locale", "es_MX");
            request.Content = new StringContent(JsonSerializer.Serialize(peticionFedex), Encoding.UTF8, "application/json");

            var respuesta = await _httpClient.SendAsync(request);
            var contentType = respuesta.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var contentBytes = await respuesta.Content.ReadAsByteArrayAsync();

            string? filePath = null;
            // Guarda el PDF si es PDF
            if (contentType.Contains("pdf"))
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "ArchivosFedex");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = $"Etiqueta_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                filePath = Path.Combine(folderPath, fileName);

                await File.WriteAllBytesAsync(filePath, contentBytes);
            }

            return (contentBytes, contentType, filePath);
        }
    }
}