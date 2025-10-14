using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManyBoxApi.Models
{
    public class DhlUpdatePickupRequest
    {
        [JsonPropertyName("dispatchConfirmationNumber")]
        public string DispatchConfirmationNumber { get; set; }

        [JsonPropertyName("originalShipperAccountNumber")]
        public string OriginalShipperAccountNumber { get; set; }

        [JsonPropertyName("pickupDate")]
        public string PickupDate { get; set; }

        [JsonPropertyName("closeTime")]
        public string? CloseTime { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("locationType")]
        public string? LocationType { get; set; }

        [JsonPropertyName("accounts")]
        public List<ManyBoxApi.Models.Pickup.Account> Accounts { get; set; }

        [JsonPropertyName("specialInstructions")]
        public List<ManyBoxApi.Models.Pickup.SpecialInstruction>? SpecialInstructions { get; set; }

        [JsonPropertyName("remark")]
        public string? Remark { get; set; }

        [JsonPropertyName("customerDetails")]
        public ManyBoxApi.Models.Pickup.CustomerDetails CustomerDetails { get; set; }

        [JsonPropertyName("shipmentDetails")]
        public List<ManyBoxApi.Models.Pickup.ShipmentDetail>? ShipmentDetails { get; set; }
    }
}
