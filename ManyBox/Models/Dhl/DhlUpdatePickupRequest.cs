using System.Collections.Generic;

namespace ManyBox.Models.Dhl
{
    public class DhlUpdatePickupRequest
    {
        public string DispatchConfirmationNumber { get; set; } = string.Empty;
        public string OriginalShipperAccountNumber { get; set; } = string.Empty;
        public string PickupDate { get; set; } = string.Empty;
        public string? CloseTime { get; set; }
        public string? Location { get; set; }
        public string? LocationType { get; set; }
        public List<Account> Accounts { get; set; } = new();
        public List<SpecialInstruction>? SpecialInstructions { get; set; }
        public string? Remark { get; set; }
        public CustomerDetails CustomerDetails { get; set; } = new();
        public List<ShipmentDetail>? ShipmentDetails { get; set; } = new();
    }
}
