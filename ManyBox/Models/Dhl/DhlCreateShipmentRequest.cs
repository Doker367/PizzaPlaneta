using System.Collections.Generic;

namespace ManyBox.Models.Dhl
{
    public class DhlCreateShipmentRequest
    {
        public string PlannedShippingDateAndTime { get; set; } = string.Empty;
        public ShipmentPickup Pickup { get; set; } = new();
        public string ProductCode { get; set; } = string.Empty;
        public List<Account> Accounts { get; set; } = new();
        public CustomerDetails CustomerDetails { get; set; } = new();
        public Content Content { get; set; } = new();
    }

    public class ShipmentPickup
    {
        public bool IsRequested { get; set; }
    }

    public class Content
    {
        public List<DhlPackage> Packages { get; set; } = new();
        public bool IsCustomsDeclarable { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Incoterm { get; set; } = string.Empty;
        public string UnitOfMeasurement { get; set; } = string.Empty;
    }

    public class DhlPackage
    {
        public decimal Weight { get; set; }
        public DhlDimensionsDecimal Dimensions { get; set; } = new();
    }
}
