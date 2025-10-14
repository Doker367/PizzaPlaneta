using System;
using System.Collections.Generic;

namespace ManyBoxApi.Models
{
    public class FedexShipRequest
    {
        public string? Index { get; set; }
        public RequestedShipment? RequestedShipment { get; set; }
        public string? OpenShipmentAction { get; set; }
        public AccountNumber? AccountNumber { get; set; }
    }

    public class RequestedShipment
    {
        public string? ShipDatestamp { get; set; }
        public string? PickupType { get; set; }
        public string? ServiceType { get; set; }
        public string? PackagingType { get; set; }
        public double? TotalWeight { get; set; }
        public Party? Shipper { get; set; }
        public List<Party>? Recipients { get; set; } = new();
        public Party? SoldTo { get; set; }
        public Party? Origin { get; set; }
        public ShippingChargesPayment? ShippingChargesPayment { get; set; }
        public ShipmentSpecialServices? ShipmentSpecialServices { get; set; }
        public EmailNotificationDetail? EmailNotificationDetail { get; set; }
        public ExpressFreightDetail? ExpressFreightDetail { get; set; }
        public VariableHandlingChargeDetail? VariableHandlingChargeDetail { get; set; }
        public CustomsClearanceDetail? CustomsClearanceDetail { get; set; }
        public SmartPostInfoDetail? SmartPostInfoDetail { get; set; }
        public bool? BlockInsightVisibility { get; set; }
        public List<string>? RateRequestType { get; set; } = new();
        public string? PreferredCurrency { get; set; }
        public List<RequestedPackageLineItem>? RequestedPackageLineItems { get; set; } = new();
    }

    public class AccountNumber
    {
        public string? Value { get; set; }
    }

    public class Party
    {
        public Address? Address { get; set; }
        public Contact? Contact { get; set; }
        public List<Tin>? Tins { get; set; } = new();
        public string? DeliveryInstructions { get; set; }
        public AccountNumber? AccountNumber { get; set; }
        public string? FaxNumber { get; set; }
    }

    public class Address
    {
        public List<string>? StreetLines { get; set; } = new();
        public string? City { get; set; }
        public string? StateOrProvinceCode { get; set; }
        public string? PostalCode { get; set; }
        public string? CountryCode { get; set; }
        public bool? Residential { get; set; }
    }

    public class Contact
    {
        public string? PersonName { get; set; }
        public string? EmailAddress { get; set; }
        public string? PhoneExtension { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CompanyName { get; set; }
        public string? FaxNumber { get; set; }
    }

    public class Tin
    {
        public string? Number { get; set; }
        public string? TinType { get; set; }
        public string? Usage { get; set; }
        public string? EffectiveDate { get; set; }
        public string? ExpirationDate { get; set; }
    }

    public class ShippingChargesPayment
    {
        public string? PaymentType { get; set; }
        public Payor? Payor { get; set; }
    }

    public class Payor
    {
        public ResponsibleParty? ResponsibleParty { get; set; }
    }

    public class ResponsibleParty
    {
        public Address? Address { get; set; }
        public Contact? Contact { get; set; }
        public AccountNumber? AccountNumber { get; set; }
        public List<Tin>? Tins { get; set; } = new();
    }

    public class ShipmentSpecialServices
    {
        public List<string>? SpecialServiceTypes { get; set; } = new();
        // Agrega m�s campos si los necesitas (Opcional)
    }

    public class EmailNotificationDetail
    {
        public string? AggregationType { get; set; }
        public List<EmailNotificationRecipient>? EmailNotificationRecipients { get; set; } = new();
        public string? PersonalMessage { get; set; }
    }

    public class EmailNotificationRecipient
    {
        public string? Name { get; set; }
        public string? EmailNotificationRecipientType { get; set; }
        public string? EmailAddress { get; set; }
        public string? NotificationFormatType { get; set; }
        public string? NotificationType { get; set; }
        public string? Locale { get; set; }
        public List<string>? NotificationEventType { get; set; } = new();
    }

    public class ExpressFreightDetail
    {
        public string? BookingConfirmationNumber { get; set; }
        public int? ShippersLoadAndCount { get; set; }
        public bool? PackingListEnclosed { get; set; }
    }

    public class VariableHandlingChargeDetail
    {
        public string? RateType { get; set; }
        public double? PercentValue { get; set; }
        public string? RateLevelType { get; set; }
        public FixedValue? FixedValue { get; set; }
        public string? RateElementBasis { get; set; }
    }

    public class FixedValue
    {
        public double? Amount { get; set; }
        public string? Currency { get; set; }
    }

    public class CustomsClearanceDetail
    {
        // Define aqu� los campos necesarios
    }

    public class SmartPostInfoDetail
    {
        public string? AncillaryEndorsement { get; set; }
        public string? HubId { get; set; }
        public string? Indicia { get; set; }
        public string? SpecialServices { get; set; }
    }

    public class RequestedPackageLineItem
    {
        public string? SequenceNumber { get; set; }
        public string? SubPackagingType { get; set; }
        public List<CustomerReference>? CustomerReferences { get; set; } = new();
        public Amount? DeclaredValue { get; set; }
        public FedexWeight? Weight { get; set; }
        public Dimensions? Dimensions { get; set; }
        public int? GroupPackageCount { get; set; }
        public string? ItemDescriptionForClearance { get; set; }
        public List<ContentRecord>? ContentRecord { get; set; } = new();
        public string? ItemDescription { get; set; }
        public VariableHandlingChargeDetail? VariableHandlingChargeDetail { get; set; }
        public PackageSpecialServices? PackageSpecialServices { get; set; }
    }

    public class CustomerReference
    {
        public string? CustomerReferenceType { get; set; }
        public string? Value { get; set; }
    }

    public class Amount
    {
        public double? AmountValue { get; set; }
        public string? Currency { get; set; }
    }

    public class FedexWeight
    {
        public string? Units { get; set; }
        public double? Value { get; set; }
    }

    public class Dimensions
    {
        public int? Length { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string? Units { get; set; }
    }

    public class ContentRecord
    {
        public string? ItemNumber { get; set; }
        public int? ReceivedQuantity { get; set; }
        public string? Description { get; set; }
        public string? PartNumber { get; set; }
    }

    public class PackageSpecialServices
    {
        public List<string>? SpecialServiceTypes { get; set; } = new();
        public string? SignatureOptionType { get; set; }
        // Agrega m�s campos si los necesitas
    }
}