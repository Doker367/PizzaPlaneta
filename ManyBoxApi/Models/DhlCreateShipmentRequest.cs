using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManyBoxApi.Models
{
    public class DhlCreateShipmentRequest
    {
        [JsonPropertyName("plannedShippingDateAndTime")]
        public required string PlannedShippingDateAndTime { get; set; }

        [JsonPropertyName("pickup")]
        public required ShipmentPickup Pickup { get; set; }

        [JsonPropertyName("productCode")]
        public required string ProductCode { get; set; }

        [JsonPropertyName("accounts")]
        public required List<Account> Accounts { get; set; }

        [JsonPropertyName("customerDetails")]
        public required CustomerDetails CustomerDetails { get; set; }

        [JsonPropertyName("content")]
        public required Content Content { get; set; }

        [JsonPropertyName("outputImageProperties")]
        public OutputImageProperties? OutputImageProperties { get; set; }
    }

    public class ShipmentPickup
    {
        [JsonPropertyName("isRequested")]
        public bool IsRequested { get; set; }
    }

    public class Account
    {
        [JsonPropertyName("typeCode")]
        public required string TypeCode { get; set; }

        [JsonPropertyName("number")]
        public required string Number { get; set; }
    }

    public class CustomerDetails
    {
        [JsonPropertyName("shipperDetails")]
        public required ShipperDetails ShipperDetails { get; set; }

        [JsonPropertyName("receiverDetails")]
        public required ReceiverDetails ReceiverDetails { get; set; }
    }

    public class ShipperDetails
    {
        [JsonPropertyName("postalAddress")]
        public required PostalAddress PostalAddress { get; set; }

        [JsonPropertyName("contactInformation")]
        public required ContactInformation ContactInformation { get; set; }
    }

    public class ReceiverDetails
    {
        [JsonPropertyName("postalAddress")]
        public required PostalAddress PostalAddress { get; set; }

        [JsonPropertyName("contactInformation")]
        public required ContactInformation ContactInformation { get; set; }
    }

    public class PostalAddress
    {
        [JsonPropertyName("postalCode")]
        public required string PostalCode { get; set; }

        [JsonPropertyName("cityName")]
        public required string CityName { get; set; }

        [JsonPropertyName("countryCode")]
        public required string CountryCode { get; set; }

        [JsonPropertyName("addressLine1")]
        public required string AddressLine1 { get; set; }

        [JsonPropertyName("addressLine2")]
        public string? AddressLine2 { get; set; }

        [JsonPropertyName("addressLine3")]
        public string? AddressLine3 { get; set; }

        [JsonPropertyName("countyName")]
        public string? CountyName { get; set; }
    }

    public class ContactInformation
    {
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("phone")]
        public required string Phone { get; set; }

        [JsonPropertyName("companyName")]
        public required string CompanyName { get; set; }

        [JsonPropertyName("fullName")]
        public required string FullName { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("packages")]
        public required List<DhlPackage> Packages { get; set; }

        [JsonPropertyName("isCustomsDeclarable")]
        public bool IsCustomsDeclarable { get; set; }

        [JsonPropertyName("description")]
        public required string Description { get; set; }

        [JsonPropertyName("incoterm")]
        public required string Incoterm { get; set; }

        [JsonPropertyName("unitOfMeasurement")]
        public required string UnitOfMeasurement { get; set; }

        [JsonPropertyName("declaredValue")]
        public required double DeclaredValue { get; set; }

        [JsonPropertyName("declaredValueCurrency")]
        public required string DeclaredValueCurrency { get; set; }

        [JsonPropertyName("exportDeclaration")]
        public ExportDeclaration? ExportDeclaration { get; set; }
    }

    public class DhlPackage
    {
        [JsonPropertyName("weight")]
        public required double Weight { get; set; }

        [JsonPropertyName("dimensions")]
        public required DhlDimensionsDecimal Dimensions { get; set; }
    }

    public class OutputImageProperties
    {
        [JsonPropertyName("printerDPI")]
        public int? PrinterDPI { get; set; }

        [JsonPropertyName("encodingFormat")]
        public string? EncodingFormat { get; set; }

        [JsonPropertyName("imageOptions")]
        public List<ImageOption>? ImageOptions { get; set; }
    }

    public class ImageOption
    {
        [JsonPropertyName("typeCode")]
        public required string TypeCode { get; set; } // "WAYBILL_DOCUMENT", "COMMERCIAL_INVOICE", etc.

        [JsonPropertyName("templateName")]
        public string? TemplateName { get; set; }

        [JsonPropertyName("isRequested")]
        public bool IsRequested { get; set; } = true;
    }

    public class ExportDeclaration
    {
        [JsonPropertyName("lineItems")]
        public required List<LineItem> LineItems { get; set; }

        [JsonPropertyName("invoice")]
        public required Invoice Invoice { get; set; }

        [JsonPropertyName("remarks")]
        public List<Remark>? Remarks { get; set; }

        [JsonPropertyName("additionalCharges")]
        public List<AdditionalCharge>? AdditionalCharges { get; set; }

        [JsonPropertyName("exportReasonType")]
        public string? ExportReasonType { get; set; } // "permanent", "temporary", "re-export"
        
        [JsonPropertyName("placeOfIncoterm")]
        public string? PlaceOfIncoterm { get; set; }
    }

    public class LineItem
    {
        [JsonPropertyName("number")]
        public required int Number { get; set; }

        [JsonPropertyName("description")]
        public required string Description { get; set; }

        [JsonPropertyName("price")]
        public required double Price { get; set; }

        [JsonPropertyName("quantity")]
        public required Quantity Quantity { get; set; }

        [JsonPropertyName("commodityCodes")]
        public List<CommodityCode>? CommodityCodes { get; set; }

        [JsonPropertyName("exportReasonType")]
        public string? ExportReasonType { get; set; } // "permanent", "temporary", "return"

        [JsonPropertyName("manufacturerCountry")]
        public required string ManufacturerCountry { get; set; }

        [JsonPropertyName("weight")]
        public required DhlWeight Weight { get; set; }

        [JsonPropertyName("isTaxesPaid")]
        public bool? IsTaxesPaid { get; set; }
    }

    public class Invoice
    {
        [JsonPropertyName("number")]
        public required string Number { get; set; }

        [JsonPropertyName("date")]
        public required string Date { get; set; }
        
        [JsonPropertyName("signatureName")]
        public string? SignatureName { get; set; }
        
        [JsonPropertyName("signatureTitle")]
        public string? SignatureTitle { get; set; }
        
        [JsonPropertyName("instructions")]
        public List<string>? Instructions { get; set; }
    }

    public class Quantity
    {
        [JsonPropertyName("value")]
        public required int Value { get; set; }

        [JsonPropertyName("unitOfMeasurement")]
        public required string UnitOfMeasurement { get; set; }
    }

    public class CommodityCode
    {
        [JsonPropertyName("typeCode")]
        public required string TypeCode { get; set; } // "outbound", "inbound"

        [JsonPropertyName("value")]
        public required string Value { get; set; }
    }

    public class DhlWeight
    {
        [JsonPropertyName("netValue")]
        public required double NetValue { get; set; }

        [JsonPropertyName("grossValue")]
        public required double GrossValue { get; set; }
    }
    
    public class Remark
    {
        [JsonPropertyName("value")]
        public required string Value { get; set; }
    }
    
    public class AdditionalCharge
    {
        [JsonPropertyName("value")]
        public required double Value { get; set; }
        
        [JsonPropertyName("typeCode")]
        public required string TypeCode { get; set; }
    }
}