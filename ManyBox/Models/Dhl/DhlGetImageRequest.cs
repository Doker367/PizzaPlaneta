namespace ManyBox.Models.Dhl
{
    public class DhlGetImageRequest
    {
        public string ShipperAccountNumber { get; set; } = string.Empty;
        public string TypeCode { get; set; } = string.Empty;
        public string PickupYearAndMonth { get; set; } = string.Empty;
        public string? EncodingFormat { get; set; }
        public bool? AllInOnePDF { get; set; }
        public bool? CompressedPackage { get; set; }
    }
}
