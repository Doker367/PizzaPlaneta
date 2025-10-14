using SQLite;
using System;

namespace ManyBox.Models.Locals
{
    [Table("Paquetes")]
    public class PaquetesLocal
    {
        [PrimaryKey]
        [Column("IdPaqueteAsociado")]
        public Guid IdPaqueteAsociado { get; set; } = Guid.NewGuid();

        [Column("IdPaquete")]
        public Guid IdPaquete { get; set; } = Guid.NewGuid();

        // Relaciones con remitente y destinatario
        [Indexed]
        public Guid IdRemitenteAsociado { get; set; }

        [Indexed]
        public Guid IdDestinatarioAsociado { get; set; }

        // Propiedades del paquete
        [Column("PesoKg")]
        public double PesoKg { get; set; }

        [Column("VolumenM3")]
        public double VolumenM3 { get; set; }

        [Column("DescripcionContenido")]
        public string? DescripcionContenido { get; set; }

        [Column("EstatusPaquete")]
        public string? EstatusPaquete { get; set; } // Ej: "En tránsito", "Entregado", "Pendiente de entrega", "Cancelado"

        [Column("FechaRegistro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // NUEVOS CAMPOS
        [Column("CompaniaEnvio")]
        public string? CompaniaEnvio { get; set; }

        [Column("TieneSeguro")]
        public bool TieneSeguro { get; set; }

        [Column("MontoSeguro")]
        public decimal? MontoSeguro { get; set; }
    }
}