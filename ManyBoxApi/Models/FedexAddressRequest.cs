using Microsoft.AspNetCore.Builder;

namespace ManyBoxApi.Models
{
    public class FedexAddressRequest
    {
        public string[] StreetLines { get; set; } = Array.Empty<string>();
        public string City { get; set; } = string.Empty;
        public string StateOrProvinceCode { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
    }
}
