using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManyBoxApi.Models.Pickup
{
    // Request Models
    public class PickupRequest
    {
        [JsonPropertyName("pickupDate")]
        public string PickupDate { get; set; }

        [JsonPropertyName("closeTime")]
        public string? CloseTime { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("locationType")]
        public string? LocationType { get; set; }

        [JsonPropertyName("accounts")]
        public List<Account> Accounts { get; set; }

        [JsonPropertyName("specialInstructions")]
        public List<SpecialInstruction>? SpecialInstructions { get; set; }

        [JsonPropertyName("remark")]
        public string? Remark { get; set; }

        [JsonPropertyName("customerDetails")]
        public CustomerDetails CustomerDetails { get; set; }

        [JsonPropertyName("shipmentDetails")]
        public List<ShipmentDetail> ShipmentDetails { get; set; }
    }

    public class Account
    {
        [JsonPropertyName("typeCode")]
        public string TypeCode { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }
    }

    public class SpecialInstruction
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("typeCode")]
        public string? TypeCode { get; set; }
    }

    public class CustomerDetails
    {
        [JsonPropertyName("shipperDetails")]
        public PartyDetails ShipperDetails { get; set; }

        [JsonPropertyName("receiverDetails")]
        public PartyDetails? ReceiverDetails { get; set; }

        [JsonPropertyName("bookingRequestorDetails")]
        public PartyDetails? BookingRequestorDetails { get; set; }

        [JsonPropertyName("pickupDetails")]
        public PartyDetails? PickupDetails { get; set; }
    }

    public class PartyDetails
    {
        [JsonPropertyName("postalAddress")]
        public Address PostalAddress { get; set; }

        [JsonPropertyName("contactInformation")]
        public Contact ContactInformation { get; set; }
    }

    public class Address
    {
        [JsonPropertyName("postalCode")]
        public string PostalCode { get; set; }

        [JsonPropertyName("cityName")]
        public string CityName { get; set; }

        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; }

        [JsonPropertyName("provinceCode")]
        public string? ProvinceCode { get; set; }

        [JsonPropertyName("addressLine1")]
        public string AddressLine1 { get; set; }

        [JsonPropertyName("addressLine2")]
        public string? AddressLine2 { get; set; }

        [JsonPropertyName("addressLine3")]
        public string? AddressLine3 { get; set; }

        [JsonPropertyName("countyName")]
        public string? CountyName { get; set; }
    }

    public class Contact
    {
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("mobilePhone")]
        public string? MobilePhone { get; set; }

        [JsonPropertyName("companyName")]
        public string CompanyName { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; }
    }

    public class ShipmentDetail
    {
        [JsonPropertyName("productCode")]
        public string ProductCode { get; set; }

        [JsonPropertyName("localProductCode")]
        public string? LocalProductCode { get; set; }

        [JsonPropertyName("accounts")]
        public List<Account>? Accounts { get; set; }

        [JsonPropertyName("valueAddedServices")]
        public List<ValueAddedService>? ValueAddedServices { get; set; }

        [JsonPropertyName("isCustomsDeclarable")]
        public bool IsCustomsDeclarable { get; set; }

        [JsonPropertyName("declaredValue")]
        public decimal? DeclaredValue { get; set; }

        [JsonPropertyName("declaredValueCurrency")]
        public string? DeclaredValueCurrency { get; set; }

        [JsonPropertyName("unitOfMeasurement")]
        public string UnitOfMeasurement { get; set; }

        [JsonPropertyName("shipmentTrackingNumber")]
        public string? ShipmentTrackingNumber { get; set; }

        [JsonPropertyName("packages")]
        public List<Package> Packages { get; set; }
    }

    public class ValueAddedService
    {
        [JsonPropertyName("serviceCode")]
        public string ServiceCode { get; set; }

        [JsonPropertyName("localServiceCode")]
        public string? LocalServiceCode { get; set; }

        [JsonPropertyName("value")]
        public decimal? Value { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }
    }

    public class Package
    {
        [JsonPropertyName("typeCode")]
        public string? TypeCode { get; set; }

        [JsonPropertyName("weight")]
        public decimal Weight { get; set; }

        [JsonPropertyName("dimensions")]
        public Dimensions Dimensions { get; set; }
    }

    public class Dimensions
    {
        [JsonPropertyName("length")]
        public decimal Length { get; set; }

        [JsonPropertyName("width")]
        public decimal Width { get; set; }

        [JsonPropertyName("height")]
        public decimal Height { get; set; }
    }

    // Response Model
    public class PickupResponse
    {
        [JsonPropertyName("dispatchConfirmationNumbers")]
        public List<string> DispatchConfirmationNumbers { get; set; }

        [JsonPropertyName("readyByTime")]
        public string ReadyByTime { get; set; }

        [JsonPropertyName("nextPickupDate")]
        public string NextPickupDate { get; set; }

        [JsonPropertyName("warnings")]
        public List<string> Warnings { get; set; }
    }
}
