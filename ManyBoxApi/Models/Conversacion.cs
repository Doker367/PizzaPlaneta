using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManyBoxApi.Models
{
    [Table("conversaciones")]
    public class Conversacion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // "directa" | "grupo"
        [Column("tipo")]
        public string Tipo { get; set; } = "directa";

        [Column("nombre")]
        public string? Nombre { get; set; }

        [Column("creado_por")]
        public int CreadoPor { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("Archivada")]
        public bool Archivada { get; set; }

        public ICollection<ConversacionParticipante> Participantes { get; set; } = new List<ConversacionParticipante>();
        public ICollection<MensajeChat> Mensajes { get; set; } = new List<MensajeChat>();
    }
}
