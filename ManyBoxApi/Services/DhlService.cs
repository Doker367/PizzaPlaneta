using ManyBoxApi.Models;
using ManyBoxApi.Models.Pickup;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ManyBoxApi.Services
{
    public class DhlService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly JsonSerializerOptions _jsonOptions;

        public DhlService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _jsonOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<JsonElement> GetRatesAsync(DhlRateRequest request)
        {
            var apiUrl = _config["Dhl:ApiUrl"];
            // Construir el query string manualmente para los decimales
            var query = $"accountNumber={Uri.EscapeDataString(request.AccountNumber)}"
                        + $"&originCountryCode={Uri.EscapeDataString(request.OriginCountryCode)}"
                        + $"&originCityName={Uri.EscapeDataString(request.OriginCityName)}"
                        + $"&destinationCountryCode={Uri.EscapeDataString(request.DestinationCountryCode)}"
                        + $"&destinationCityName={Uri.EscapeDataString(request.DestinationCityName)}"
                        + $"&weight={request.Weight.ToString(CultureInfo.InvariantCulture)}"
                        + $"&length={request.Length.ToString(CultureInfo.InvariantCulture)}"
                        + $"&width={request.Width.ToString(CultureInfo.InvariantCulture)}"
                        + $"&height={request.Height.ToString(CultureInfo.InvariantCulture)}"
                        + $"&plannedShippingDate={Uri.EscapeDataString(request.PlannedShippingDate)}"
                        + $"&isCustomsDeclarable={request.IsCustomsDeclarable.ToString().ToLower()}"
                        + $"&unitOfMeasurement={Uri.EscapeDataString(request.UnitOfMeasurement)}";

            if (!string.IsNullOrEmpty(request.OriginPostalCode))
                query += $"&originPostalCode={Uri.EscapeDataString(request.OriginPostalCode)}";
            if (!string.IsNullOrEmpty(request.DestinationPostalCode))
                query += $"&destinationPostalCode={Uri.EscapeDataString(request.DestinationPostalCode)}";
            if (request.NextBusinessDay.HasValue)
                query += $"&nextBusinessDay={request.NextBusinessDay.Value.ToString().ToLower()}";

            var requestUrl = $"{apiUrl}/rates?{query}";
            // Log temporal para depuración
            Console.WriteLine($"DHL Request URL: {requestUrl}");
            var responseString = await SendDhlRequestAsync(HttpMethod.Get, requestUrl);
            return JsonSerializer.Deserialize<JsonElement>(responseString, _jsonOptions);
        }

        public async Task<DhlCreateShipmentResponse> CreateShipmentAsync(DhlCreateShipmentRequest request)
        {
            var requestUrl = $"{_config["Dhl:ApiUrl"]}/shipments";

            // Reformat date to DHL's expected format
            if (DateTimeOffset.TryParse(request.PlannedShippingDateAndTime, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset parsedDate))
            {
                request.PlannedShippingDateAndTime = parsedDate.ToString("yyyy-MM-dd'T'HH:mm:ss 'GMT'zzz", CultureInfo.InvariantCulture);
            }

            if (request.Content.Description.Length > 70)
            {
                request.Content.Description = request.Content.Description.Substring(0, 70);
            }

            var jsonRequest = JsonSerializer.Serialize(request, _jsonOptions);
            Console.WriteLine($"DHL Request JSON: {jsonRequest}"); // Debugging line

            var responseString = await SendDhlRequestAsync(HttpMethod.Post, requestUrl, new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
            
            return JsonSerializer.Deserialize<DhlCreateShipmentResponse>(responseString, _jsonOptions)!;
        }

        public async Task<DhlTrackingResponse> TrackShipmentAsync(string trackingNumbers)
        {
            var requestUrl = $"{_config["Dhl:ApiUrl"]}/tracking?shipmentTrackingNumber={Uri.EscapeDataString(trackingNumbers)}";
            var responseString = await SendDhlRequestAsync(HttpMethod.Get, requestUrl);
            return JsonSerializer.Deserialize<DhlTrackingResponse>(responseString, _jsonOptions)!;
        }

        public async Task<PickupResponse> CreatePickupAsync(PickupRequest request)
        {
            var requestUrl = $"{_config["Dhl:ApiUrl"]}/pickups";

            // Convert PickupDate to ISO format if needed
            if (DateTime.TryParse(request.PickupDate, out DateTime pickupDate))
            {
                request.PickupDate = pickupDate.ToString("yyyy-MM-dd");
            }

            var jsonRequest = JsonSerializer.Serialize(request, _jsonOptions);
            var responseString = await SendDhlRequestAsync(HttpMethod.Post, requestUrl, new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
            return JsonSerializer.Deserialize<PickupResponse>(responseString, _jsonOptions)!;
        }

        public async Task<DhlAddressValidateResponse> ValidateAddressAsync(DhlAddressValidateRequest request)
        {
            var apiUrl = _config["Dhl:ApiUrl"];
            var queryParams = new Dictionary<string, string>
            {
                { "type", request.Type },
                { "countryCode", request.CountryCode }
            };

            if (!string.IsNullOrEmpty(request.PostalCode))
            {
                queryParams["postalCode"] = request.PostalCode;
            }
            if (!string.IsNullOrEmpty(request.CityName))
            {
                queryParams["cityName"] = request.CityName;
            }
            if (!string.IsNullOrEmpty(request.CountyName))
            {
                queryParams["countyName"] = request.CountyName;
            }
            if (request.StrictValidation.HasValue)
            {
                queryParams["strictValidation"] = request.StrictValidation.Value.ToString().ToLower();
            }

            var queryString = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            var requestUrl = $"{apiUrl}/address-validate?{queryString}";
            var responseString = await SendDhlRequestAsync(HttpMethod.Get, requestUrl);
            return JsonSerializer.Deserialize<DhlAddressValidateResponse>(responseString, _jsonOptions)!;
        }

        public async Task<JsonElement> GetLandedCostAsync(DhlLandedCostRequest request)
        {
            var requestUrl = $"{_config["Dhl:ApiUrl"]}/landed-cost";
            var jsonRequest = JsonSerializer.Serialize(request, _jsonOptions);
            var responseString = await SendDhlRequestAsync(HttpMethod.Post, requestUrl, new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
            return JsonSerializer.Deserialize<JsonElement>(responseString, _jsonOptions);
        }

        public async Task CancelPickupAsync(string dispatchConfirmationNumber, string requestorName, string reason)
        {
            var apiUrl = _config["Dhl:ApiUrl"];
            var requestUrl = $"{apiUrl}/pickups/{dispatchConfirmationNumber}?requestorName={Uri.EscapeDataString(requestorName)}&reason={Uri.EscapeDataString(reason)}";
            await SendDhlRequestAsync(HttpMethod.Delete, requestUrl);
        }

        public async Task<DhlProofOfDeliveryResponse> GetProofOfDeliveryAsync(string shipmentTrackingNumber, string? shipperAccountNumber, string? content)
        {
            var apiUrl = _config["Dhl:ApiUrl"];
            var queryParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(shipperAccountNumber))
            {
                queryParams["shipperAccountNumber"] = shipperAccountNumber;
            }
            if (!string.IsNullOrEmpty(content))
            {
                queryParams["content"] = content;
            }

            var queryString = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            var requestUrl = $"{apiUrl}/shipments/{shipmentTrackingNumber}/proof-of-delivery";
            if (queryParams.Any())
            {
                requestUrl += $"?{queryString}";
            }
            
            var responseString = await SendDhlRequestAsync(HttpMethod.Get, requestUrl);
            return JsonSerializer.Deserialize<DhlProofOfDeliveryResponse>(responseString, _jsonOptions)!;
        }

        public async Task<DhlGetImageResponse> GetImageAsync(string shipmentTrackingNumber, DhlGetImageRequest request)
        {
            var apiUrl = _config["Dhl:ApiUrl"];
            var queryParams = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(request.ShipperAccountNumber))
            {
                queryParams["shipperAccountNumber"] = request.ShipperAccountNumber;
            }
            if (!string.IsNullOrEmpty(request.TypeCode))
            {
                queryParams["typeCode"] = request.TypeCode;
            }
            if (!string.IsNullOrEmpty(request.PickupYearAndMonth))
            {
                queryParams["pickupYearAndMonth"] = request.PickupYearAndMonth;
            }
            if (!string.IsNullOrEmpty(request.EncodingFormat))
            {
                queryParams["encodingFormat"] = request.EncodingFormat;
            }
            if (request.AllInOnePDF.HasValue)
            {
                queryParams["allInOnePDF"] = request.AllInOnePDF.Value.ToString().ToLower();
            }
            if (request.CompressedPackage.HasValue)
            {
                queryParams["compressedPackage"] = request.CompressedPackage.Value.ToString().ToLower();
            }

            var queryString = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            var requestUrl = $"{apiUrl}/shipments/{shipmentTrackingNumber}/get-image";
            if (queryParams.Any())
            {
                requestUrl += $"?{queryString}";
            }
            var responseString = await SendDhlRequestAsync(HttpMethod.Get, requestUrl);
            return JsonSerializer.Deserialize<DhlGetImageResponse>(responseString, _jsonOptions)!;
        }

        public async Task<DhlUpdatePickupResponse> UpdatePickupAsync(string dispatchConfirmationNumber, DhlUpdatePickupRequest request)
        {
            var requestUrl = $"{_config["Dhl:ApiUrl"]}/pickups/{dispatchConfirmationNumber}";

            // Convert PickupDate to ISO format if needed
            if (DateTime.TryParse(request.PickupDate, out DateTime pickupDate))
            {
                request.PickupDate = pickupDate.ToString("yyyy-MM-dd");
            }

            var jsonRequest = JsonSerializer.Serialize(request, _jsonOptions);
            var responseString = await SendDhlRequestAsync(HttpMethod.Patch, requestUrl, new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
            return JsonSerializer.Deserialize<DhlUpdatePickupResponse>(responseString, _jsonOptions)!;
        }

        private async Task<string> SendDhlRequestAsync(HttpMethod method, string requestUrl, HttpContent? content = null)
        {
            var apiKey = Environment.GetEnvironmentVariable("DHL_API_KEY");
            var apiSecret = Environment.GetEnvironmentVariable("DHL_API_SECRET");

            var httpRequest = new HttpRequestMessage(method, requestUrl);
            if (content != null)
            {
                httpRequest.Content = content;
            }

            var authToken = System.Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{apiSecret}"));
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

            var response = await _httpClient.SendAsync(httpRequest);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                // Consider logging the error content
                throw new HttpRequestException($"Request to DHL API failed with status code {response.StatusCode} and content: {errorContent}");
            }
        }
    }
}