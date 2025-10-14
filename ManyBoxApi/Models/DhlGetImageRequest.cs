using System.Text.Json.Serialization;

namespace ManyBoxApi.Models
{
    public class DhlGetImageRequest
    {
        [JsonPropertyName("shipperAccountNumber")]
        public string ShipperAccountNumber { get; set; } = string.Empty;

        [JsonPropertyName("typeCode")]
        public string TypeCode { get; set; } = string.Empty; // e.g., waybill, commercial-invoice

        [JsonPropertyName("pickupYearAndMonth")]
        public string PickupYearAndMonth { get; set; } = string.Empty; // Format: YYYY-MM

        [JsonPropertyName("encodingFormat")]
        public string? EncodingFormat { get; set; } // e.g., pdf, tiff

        [JsonPropertyName("allInOnePDF")]
        public bool? AllInOnePDF { get; set; }

        [JsonPropertyName("compressedPackage")]
        public bool? CompressedPackage { get; set; }
    }
}
