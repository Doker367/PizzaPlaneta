using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Globalization;
using ManyBoxApi.Models;

namespace ManyBoxApi.Models
{
    public class DhlLandedCostRequest
    {
        [JsonPropertyName("customerDetails")]
        public required LandedCostCustomerDetails CustomerDetails { get; set; }

        [JsonPropertyName("accounts")]
        public List<Account>? Accounts { get; set; }

        [JsonPropertyName("productCode")]
        public string? ProductCode { get; set; }

        [JsonPropertyName("localProductCode")]
        public string? LocalProductCode { get; set; }

        [JsonPropertyName("unitOfMeasurement")]
        public string? UnitOfMeasurement { get; set; }

        [JsonPropertyName("currencyCode")]
        public string? CurrencyCode { get; set; }

        [JsonPropertyName("isCustomsDeclarable")]
        public bool IsCustomsDeclarable { get; set; }

        [JsonPropertyName("isDTPRequested")]
        public bool? IsDTPRequested { get; set; }

        [JsonPropertyName("isInsuranceRequested")]
        public bool? IsInsuranceRequested { get; set; }

        [JsonPropertyName("getCostBreakdown")]
        public bool GetCostBreakdown { get; set; }

        [JsonPropertyName("charges")]
        public List<Charge>? Charges { get; set; }

        [JsonPropertyName("shipmentPurpose")]
        public string? ShipmentPurpose { get; set; }

        [JsonPropertyName("transportationMode")]
        public string? TransportationMode { get; set; }

        [JsonPropertyName("merchantSelectedCarrierName")]
        public string? MerchantSelectedCarrierName { get; set; }

        [JsonPropertyName("packages")]
        public required List<DhlPackageRR> Packages { get; set; }

        [JsonPropertyName("items")]
        public required List<Item> Items { get; set; }

        [JsonPropertyName("getTariffFormula")]
        public bool? GetTariffFormula { get; set; }

        [JsonPropertyName("getQuotationID")]
        public bool? GetQuotationID { get; set; }
    }

    public class LandedCostCustomerDetails
    {
        [JsonPropertyName("shipperDetails")]
        public DhlAddressRatesRequest ShipperDetails { get; set; }

        [JsonPropertyName("receiverDetails")]
        public DhlAddressRatesRequest ReceiverDetails { get; set; }
    }

    public class DhlAddressRatesRequest
    {
        [JsonPropertyName("postalCode")]
        public string PostalCode { get; set; }

        [JsonPropertyName("cityName")]
        public string CityName { get; set; }

        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; }
    }

    public class Charge
    {
        [JsonPropertyName("typeCode")]
        public string TypeCode { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; set; }
    }

    public class Item
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("manufacturerCountry")]
        public string ManufacturerCountry { get; set; }

        [JsonPropertyName("partNumber")]
        public string? PartNumber { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("quantityType")]
        public string? QuantityType { get; set; }

        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }

        [JsonPropertyName("unitPriceCurrencyCode")]
        public string UnitPriceCurrencyCode { get; set; }

        [JsonPropertyName("commodityCode")]
        public string? CommodityCode { get; set; }

        [JsonPropertyName("weight")]
        public decimal? Weight { get; set; }

        [JsonPropertyName("weightUnitOfMeasurement")]
        public string? WeightUnitOfMeasurement { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("brand")]
        public string? Brand { get; set; }
    }

    public class DhlPackageRR
    {
        [JsonPropertyName("weight")]
        public decimal Weight { get; set; }

        [JsonPropertyName("dimensions")]
        public DhlDimensionsDecimal Dimensions { get; set; }
    }
}
