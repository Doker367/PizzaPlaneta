using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ManyBoxApi.Models;
using System.Net.Http.Headers;

public class FedexAddressValidationService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public FedexAddressValidationService(IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }

    private async Task<string> GetFedexTokenAsync()
    {
        var clientId = Environment.GetEnvironmentVariable("FEDEX_CLIENT_ID");
        var clientSecret = Environment.GetEnvironmentVariable("FEDEX_CLIENT_SECRET");
        var body = $"grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}";

        var request = new HttpRequestMessage(HttpMethod.Post, "https://apis-sandbox.fedex.com/oauth/token")
        {
            Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded")
        };

        var response = await _httpClient.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonDocument.Parse(json);
        return result.RootElement.GetProperty("access_token").GetString() ?? string.Empty;
    }

    public async Task<FedexAddressResponse> ValidateAddressAsync(FedexPostalRequest input)
    {
        try
        {
            var token = await GetFedexTokenAsync();
            var apiUrl = "https://apis-sandbox.fedex.com/country/v1/postal/validate";

            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Headers.Add("X-locale", "en_US");
            request.Content = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "";
            var json = await response.Content.ReadAsStringAsync();

            if (contentType.Contains("json") && response.IsSuccessStatusCode)
            {
                // Deserializa el JSON
                var fedexJson = JsonDocument.Parse(json);
                var fedexResponse = new FedexAddressResponse
                {
                    IsValid = true,
                    RawMessage = json // Guarda el JSON original por si lo necesitas
                };

                if (fedexJson.RootElement.TryGetProperty("transactionId", out var transId))
                    fedexResponse.TransactionId = transId.GetString();

                if (fedexJson.RootElement.TryGetProperty("output", out var outputElement))
                {
                    fedexResponse.Output = JsonSerializer.Deserialize<FedexAddressOutput>(outputElement.GetRawText());
                }

                return fedexResponse;
            }
            else
            {
                return new FedexAddressResponse
                {
                    IsValid = false,
                    RawMessage = json
                };
            }
        }
        catch (Exception ex)
        {
            return new FedexAddressResponse
            {
                IsValid = false,
                RawMessage = $"Excepción: {ex.Message}"
            };
        }
    }
}