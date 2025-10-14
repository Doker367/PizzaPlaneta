using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManyBoxApi.Models
{
    public class DhlAddressValidateResponse
    {
        [JsonPropertyName("warnings")]
        public List<string>? Warnings { get; set; }

        [JsonPropertyName("address")]
        public required List<ValidatedAddress> Address { get; set; }
    }

    public class ValidatedAddress
    {
        [JsonPropertyName("countryCode")]
        public required string CountryCode { get; set; }

        [JsonPropertyName("postalCode")]
        public required string PostalCode { get; set; }

        [JsonPropertyName("cityName")]
        public required string CityName { get; set; }

        [JsonPropertyName("countyName")]
        public string? CountyName { get; set; }

        [JsonPropertyName("serviceArea")]
        public required ServiceArea ServiceArea { get; set; }
    }

    public class ServiceArea
    {
        [JsonPropertyName("code")]
        public required string Code { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("GMTOffset")]
        public string? GMTOffset { get; set; }
    }
}
