using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManyBoxApi.Models
{
    public class DhlCreateShipmentResponse
    {
        [JsonPropertyName("shipmentTrackingNumber")]
        public string ShipmentTrackingNumber { get; set; } = string.Empty;

        [JsonPropertyName("trackingUrl")]
        public string? TrackingUrl { get; set; }

        // DHL returns per-piece tracking numbers here
        [JsonPropertyName("packages")]
        public List<DhlCreatedPackage>? Packages { get; set; }

        // DHL returns label and other documents (base64) here
        [JsonPropertyName("documents")]
        public List<DhlCreatedDocument>? Documents { get; set; }
    }

    public class DhlCreatedPackage
    {
        [JsonPropertyName("trackingNumber")]
        public string? TrackingNumber { get; set; }
    }

    public class DhlCreatedDocument
    {
        // e.g. "PDF", "ZPL"
        [JsonPropertyName("imageFormat")]
        public string? ImageFormat { get; set; }

        // base64 content
        [JsonPropertyName("content")]
        public string? Content { get; set; }

        // e.g. "label"
        [JsonPropertyName("typeCode")]
        public string? TypeCode { get; set; }

        // Sometimes present for certain docs
        [JsonPropertyName("trackingNumber")]
        public string? TrackingNumber { get; set; }
    }
}