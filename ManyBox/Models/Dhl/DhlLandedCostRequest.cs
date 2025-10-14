using System.Collections.Generic;

namespace ManyBox.Models.Dhl
{
    public class DhlLandedCostRequest
    {
        public LandedCostCustomerDetails CustomerDetails { get; set; } = new();
        public List<Account>? Accounts { get; set; }
        public string? ProductCode { get; set; }
        public string? LocalProductCode { get; set; }
        public string? UnitOfMeasurement { get; set; }
        public string? CurrencyCode { get; set; }
        public bool IsCustomsDeclarable { get; set; }
        public bool? IsDTPRequested { get; set; }
        public bool? IsInsuranceRequested { get; set; }
        public bool GetCostBreakdown { get; set; }
        public List<Charge>? Charges { get; set; }
        public string? ShipmentPurpose { get; set; }
        public string? TransportationMode { get; set; }
        public string? MerchantSelectedCarrierName { get; set; }
        public List<DhlPackageRR> Packages { get; set; } = new();
        public List<Item> Items { get; set; } = new();
        public bool? GetTariffFormula { get; set; }
        public bool? GetQuotationID { get; set; }
    }

    public class LandedCostCustomerDetails
    {
        public DhlAddressRatesRequest ShipperDetails { get; set; } = new();
        public DhlAddressRatesRequest ReceiverDetails { get; set; } = new();
    }

    public class DhlAddressRatesRequest
    {
        public string PostalCode { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
    }

    public class Charge
    {
        public string TypeCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
    }

    public class Item
    {
        public int Number { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string ManufacturerCountry { get; set; } = string.Empty;
        public string? PartNumber { get; set; }
        public int Quantity { get; set; }
        public string? QuantityType { get; set; }
        public decimal UnitPrice { get; set; }
        public string UnitPriceCurrencyCode { get; set; } = string.Empty;
        public string? CommodityCode { get; set; }
        public decimal? Weight { get; set; }
        public string? WeightUnitOfMeasurement { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
    }

    public class DhlPackageRR
    {
        public decimal Weight { get; set; }
        public DhlDimensionsDecimal Dimensions { get; set; } = new();
    }
}
