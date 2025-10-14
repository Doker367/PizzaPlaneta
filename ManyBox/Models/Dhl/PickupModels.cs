using System;
using System.Collections.Generic;

namespace ManyBox.Models.Dhl
{
    public class PickupRequest
    {
        public string PickupDate { get; set; } = string.Empty;
        public string? CloseTime { get; set; }
        public string? Location { get; set; }
        public string? LocationType { get; set; }
        public List<Account> Accounts { get; set; } = new();
        public List<SpecialInstruction>? SpecialInstructions { get; set; }
        public string? Remark { get; set; }
        public CustomerDetails CustomerDetails { get; set; } = new();
        public List<ShipmentDetail> ShipmentDetails { get; set; } = new();
    }

    public class Account
    {
        public string TypeCode { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
    }

    public class SpecialInstruction
    {
        public string Value { get; set; } = string.Empty;
        public string? TypeCode { get; set; }
    }

    public class CustomerDetails
    {
        public PartyDetails ShipperDetails { get; set; } = new();
        public PartyDetails? ReceiverDetails { get; set; }
        public PartyDetails? BookingRequestorDetails { get; set; }
        public PartyDetails? PickupDetails { get; set; }
    }

    public class PartyDetails
    {
        public Address PostalAddress { get; set; } = new();
        public Contact ContactInformation { get; set; } = new();
    }

    public class Address
    {
        public string PostalCode { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string? ProvinceCode { get; set; }
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string? AddressLine3 { get; set; }
        public string? CountyName { get; set; }
    }

    public class Contact
    {
        public string? Email { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string? MobilePhone { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public class ShipmentDetail
    {
        public string ProductCode { get; set; } = string.Empty;
        public string? LocalProductCode { get; set; }
        public List<Account>? Accounts { get; set; }
        public List<ValueAddedService>? ValueAddedServices { get; set; }
        public bool IsCustomsDeclarable { get; set; }
        public decimal? DeclaredValue { get; set; }
        public string? DeclaredValueCurrency { get; set; }
        public string UnitOfMeasurement { get; set; } = string.Empty;
        public string? ShipmentTrackingNumber { get; set; }
        public List<Package> Packages { get; set; } = new();
    }

    public class ValueAddedService
    {
        public string ServiceCode { get; set; } = string.Empty;
        public string? LocalServiceCode { get; set; }
        public decimal? Value { get; set; }
        public string? Currency { get; set; }
    }

    public class Package
    {
        public string? TypeCode { get; set; }
        public decimal Weight { get; set; }
        public DhlDimensionsDecimal Dimensions { get; set; } = new();
    }
}
