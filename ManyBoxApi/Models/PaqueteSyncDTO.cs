using System;

namespace ManyBoxApi.Models
{
    public class PaqueteSyncDTO
    {
        public Guid IdPaqueteAsociado { get; set; }
        public Guid IdPaquete { get; set; }
        public Guid IdRemitenteAsociado { get; set; }
        public Guid IdDestinatarioAsociado { get; set; }
        public double PesoKg { get; set; }
        public double VolumenM3 { get; set; }
        public string? DescripcionContenido { get; set; }
        public string? EstatusPaquete { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string? CompaniaEnvio { get; set; }
        public bool TieneSeguro { get; set; }
        public decimal? MontoSeguro { get; set; }
    }
}