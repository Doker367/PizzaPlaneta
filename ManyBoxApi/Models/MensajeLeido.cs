using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManyBoxApi.Models
{
    [Table("mensajesleidos")]
    public class MensajeLeido
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("mensaje_id")]
        public long MensajeId { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        // "enviado" | "recibido" | "leido"
        [Column("estado")]
        public string Estado { get; set; } = "enviado";

        [Column("fecha_estado")]
        public DateTime FechaEstado { get; set; } = DateTime.UtcNow;
    }
}