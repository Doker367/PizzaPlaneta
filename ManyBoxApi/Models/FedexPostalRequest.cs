namespace ManyBoxApi.Models
{
    public class FedexPostalRequest
    {
        public string carrierCode { get; set; } = "FDXE";
        public string countryCode { get; set; } = string.Empty;
        public string stateOrProvinceCode { get; set; } = string.Empty;
        public string postalCode { get; set; } = string.Empty;
        public string shipDate { get; set; } = string.Empty; // Formato: YYYY-MM-DD
    }
} 