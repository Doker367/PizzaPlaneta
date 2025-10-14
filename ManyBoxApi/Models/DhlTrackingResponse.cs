
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManyBoxApi.Models
{
    public class DhlTrackingResponse
    {
        [JsonPropertyName("shipments")]
        public List<Shipment> Shipments { get; set; }
    }

    public class Shipment
    {
        [JsonPropertyName("shipmentTrackingNumber")]
        public string ShipmentTrackingNumber { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("shipmentTimestamp")]
        public string ShipmentTimestamp { get; set; }

        [JsonPropertyName("productCode")]
        public string ProductCode { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("events")]
        public List<Event> Events { get; set; }
    }

    public class Event
    {
        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("typeCode")]
        public string TypeCode { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
