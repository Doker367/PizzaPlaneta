namespace ManyBox.Models.Dhl
{
    public class DhlRateRequest
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string OriginCountryCode { get; set; } = string.Empty;
        public string OriginCityName { get; set; } = string.Empty;
        public string DestinationCountryCode { get; set; } = string.Empty;
        public string DestinationCityName { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public DhlDimensionsDecimal Dimensions { get; set; } = new();
        public string PlannedShippingDate { get; set; } = string.Empty;
        public bool IsCustomsDeclarable { get; set; }
        public string UnitOfMeasurement { get; set; } = string.Empty;
        public string? OriginPostalCode { get; set; }
        public string? DestinationPostalCode { get; set; }
        public bool? NextBusinessDay { get; set; }
    }
}
