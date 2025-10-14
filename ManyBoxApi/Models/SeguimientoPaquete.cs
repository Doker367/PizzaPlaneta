using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManyBoxApi.Models
{
    [Table("seguimientopaquete")]
    public class SeguimientoPaquete
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("status")]
        public string Status { get; set; } = string.Empty;

        [Column("envio_id")]
        public int EnvioId { get; set; }
        public Envio Envio { get; set; }

        [Column("fecha_status")]
        public DateTime FechaStatus { get; set; } // Corregido el nombre de la propiedad
    }
}