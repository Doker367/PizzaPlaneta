using System;
using System.Collections.Generic;

namespace ManyBoxApi.Models
{
    public class FedexShipResponse
    {
        public Output? Output { get; set; }
        public List<Alert>? Alerts { get; set; }
    }

    public class Output
    {
        public string? TransactionId { get; set; }
        public List<CompletedShipmentDetail>? CompletedShipmentDetails { get; set; }
        public List<Document>? Documents { get; set; }
    }

    public class CompletedShipmentDetail
    {
        public string? MasterTrackingNumber { get; set; }
        public ShipmentRating? ShipmentRating { get; set; }
        public OperationalDetail? OperationalDetail { get; set; }
    }

    public class ShipmentRating
    {
        public decimal? ActualRateType { get; set; }
        public decimal? EffectiveNetDiscount { get; set; }
        public decimal? ShipmentRateDetail { get; set; }
    }

    public class OperationalDetail
    {
        public string? UrsaPrefixCode { get; set; }
        public string? UrsaSuffixCode { get; set; }
    }

    public class Document
    {
        public string? Type { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
        public string? Encoding { get; set; }
        public string? Url { get; set; }
        public string? Image { get; set; } // base64
    }

    public class Alert
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
        public string? AlertType { get; set; }
    }
}