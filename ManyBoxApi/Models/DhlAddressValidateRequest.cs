using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace ManyBoxApi.Models
{
    public class DhlAddressValidateRequest
    {
        [FromQuery(Name = "type")]
        public required string Type { get; set; } // "pickup" or "delivery"

        [FromQuery(Name = "countryCode")]
        public required string CountryCode { get; set; }

        [FromQuery(Name = "postalCode")]
        public string? PostalCode { get; set; }

        [FromQuery(Name = "cityName")]
        public string? CityName { get; set; }

        [FromQuery(Name = "countyName")]
        public string? CountyName { get; set; }

        [FromQuery(Name = "strictValidation")]
        public bool? StrictValidation { get; set; }
    }
}
