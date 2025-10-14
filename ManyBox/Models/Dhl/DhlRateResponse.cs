using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManyBox.Models.Dhl
{
    // Root response for /dhl/rates
    public class DhlRateResponse
    {
        [JsonPropertyName("products")] public List<DhlProduct> Products { get; set; } = new();
        [JsonPropertyName("exchangeRates")] public List<DhlExchangeRate> ExchangeRates { get; set; } = new();
    }

    public class DhlProduct
    {
        [JsonPropertyName("productName")] public string? ProductName { get; set; }
        [JsonPropertyName("productCode")] public string? ProductCode { get; set; }
        [JsonPropertyName("networkTypeCode")] public string? NetworkTypeCode { get; set; }
        [JsonPropertyName("isCustomerAgreement")] public bool IsCustomerAgreement { get; set; }
        [JsonPropertyName("weight")] public DhlWeight? Weight { get; set; }
        [JsonPropertyName("totalPrice")] public List<DhlPriceItem> TotalPrice { get; set; } = new();
        [JsonPropertyName("deliveryCapabilities")] public DhlDeliveryCapabilities? DeliveryCapabilities { get; set; }
        [JsonPropertyName("pickupCapabilities")] public DhlPickupCapabilities? PickupCapabilities { get; set; }
        [JsonPropertyName("detailedPriceBreakdown")] public List<DhlDetailedCurrencyBreakdown>? DetailedPriceBreakdown { get; set; }
    }

    public class DhlWeight
    {
        [JsonPropertyName("volumetric")] public decimal? Volumetric { get; set; }
        [JsonPropertyName("provided")] public decimal? Provided { get; set; }
        [JsonPropertyName("unitOfMeasurement")] public string? UnitOfMeasurement { get; set; }
    }

    public class DhlPriceItem
    {
        [JsonPropertyName("currencyType")] public string? CurrencyType { get; set; }
        [JsonPropertyName("priceCurrency")] public string? PriceCurrency { get; set; }
        [JsonPropertyName("price")] public decimal? Price { get; set; }
    }

    public class DhlDeliveryCapabilities
    {
        [JsonPropertyName("deliveryTypeCode")] public string? DeliveryTypeCode { get; set; }
        [JsonPropertyName("estimatedDeliveryDateAndTime")] public DateTime? EstimatedDeliveryDateAndTime { get; set; }
        [JsonPropertyName("totalTransitDays")] public int? TotalTransitDays { get; set; }
    }

    public class DhlPickupCapabilities
    {
        [JsonPropertyName("nextBusinessDay")] public bool? NextBusinessDay { get; set; }
        [JsonPropertyName("localCutoffDateAndTime")] public DateTime? LocalCutoffDateAndTime { get; set; }
        [JsonPropertyName("pickupEarliest")] public string? PickupEarliest { get; set; }
        [JsonPropertyName("pickupLatest")] public string? PickupLatest { get; set; }
    }

    public class DhlExchangeRate
    {
        [JsonPropertyName("currentExchangeRate")] public decimal? CurrentExchangeRate { get; set; }
        [JsonPropertyName("currency")] public string? Currency { get; set; }
        [JsonPropertyName("baseCurrency")] public string? BaseCurrency { get; set; }
    }

    // Optional: breakdown per currency to render quick details if needed
    public class DhlDetailedCurrencyBreakdown
    {
        [JsonPropertyName("currencyType")] public string? CurrencyType { get; set; }
        [JsonPropertyName("priceCurrency")] public string? PriceCurrency { get; set; }
        [JsonPropertyName("breakdown")] public List<DhlBreakdownItem>? Breakdown { get; set; }
    }

    public class DhlBreakdownItem
    {
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("price")] public decimal? Price { get; set; }
        [JsonPropertyName("serviceCode")] public string? ServiceCode { get; set; }
        [JsonPropertyName("serviceTypeCode")] public string? ServiceTypeCode { get; set; }
        [JsonPropertyName("priceBreakdown")] public List<DhlPriceComponent>? PriceBreakdown { get; set; }
    }

    public class DhlPriceComponent
    {
        [JsonPropertyName("priceType")] public string? PriceType { get; set; }
        [JsonPropertyName("typeCode")] public string? TypeCode { get; set; }
        [JsonPropertyName("price")] public decimal? Price { get; set; }
        [JsonPropertyName("rate")] public decimal? Rate { get; set; }
        [JsonPropertyName("basePrice")] public decimal? BasePrice { get; set; }
    }
}
