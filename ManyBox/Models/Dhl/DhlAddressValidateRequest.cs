namespace ManyBox.Models.Dhl
{
    public class DhlAddressValidateRequest
    {
        public required string Type { get; set; } // "pickup" or "delivery"
        public required string CountryCode { get; set; }
        public string? PostalCode { get; set; }
        public string? CityName { get; set; }
        public string? CountyName { get; set; }
        public bool? StrictValidation { get; set; }
    }
}
