using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManyBoxApi.Models
{
    public class DhlGetImageResponse
    {
        [JsonPropertyName("documents")]
        public required List<DhlGetImageDocument> Documents { get; set; }
    }

    public class DhlGetImageDocument
    {
        [JsonPropertyName("shipmentTrackingNumber")]
        public required string ShipmentTrackingNumber { get; set; }

        [JsonPropertyName("typeCode")]
        public required string TypeCode { get; set; }

        [JsonPropertyName("function")]
        public string? Function { get; set; }

        [JsonPropertyName("encodingFormat")]
        public required string EncodingFormat { get; set; }

        [JsonPropertyName("content")]
        public required string Content { get; set; }
    }
}
