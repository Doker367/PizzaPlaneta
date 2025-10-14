using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ManyBoxApi.Models
{
    public class DhlRateRequest
    {
        [Required]
        public string AccountNumber { get; set; }

        [Required]
        public string OriginCountryCode { get; set; }

        public string OriginPostalCode { get; set; }

        [Required]
        public string OriginCityName { get; set; }

        [Required]
        public string DestinationCountryCode { get; set; }

        public string DestinationPostalCode { get; set; }

        [Required]
        public string DestinationCityName { get; set; }

        [Required]
        public decimal Weight { get; set; }

        [Required]
        public decimal Length { get; set; }

        [Required]
        public decimal Width { get; set; }

        [Required]
        public decimal Height { get; set; }

        [Required]
        public string PlannedShippingDate { get; set; }

        [Required]
        public bool IsCustomsDeclarable { get; set; }

        [Required]
        public string UnitOfMeasurement { get; set; }

        public bool? NextBusinessDay { get; set; }
    }
}
