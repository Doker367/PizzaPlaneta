using System.Collections.Generic;

namespace ManyBoxApi.Models
{
    public class FedexAddressResponse
    {
        public bool IsValid { get; set; }
        public string? TransactionId { get; set; }
        public FedexAddressOutput? Output { get; set; }
        public string? RawMessage { get; set; } // Opcional: el JSON original
    }

    public class FedexAddressOutput
    {
        public List<FedexAlert>? Alerts { get; set; }
        public string? CleanedPostalCode { get; set; }
        public string? CountryCode { get; set; }
        public string? StateOrProvinceCode { get; set; }
        public string? CityFirstInitials { get; set; }
        public List<FedexLocationDescription>? LocationDescriptions { get; set; }
    }

    public class FedexAlert
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
        public string? AlertType { get; set; }
    }

    public class FedexLocationDescription
    {
        public string? LocationId { get; set; }
        public int? LocationNumber { get; set; }
        public string? ServiceArea { get; set; }
        public string? AirportId { get; set; }
    }
}
