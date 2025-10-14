using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManyBoxApi.Models
{
    [Table("mensajeschat")]
    public class MensajeChat
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("conversacion_id")]
        public int ConversacionId { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("mensaje", TypeName = "text")]
        public string Contenido { get; set; } = string.Empty;

        // "texto" | "imagen" | "archivo" (ajústalo a tu enum real)
        [Column("tipo")]
        public string Tipo { get; set; } = "texto";

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("editado")]
        public bool Editado { get; set; }

        [Column("eliminado")]
        public bool Eliminado { get; set; }

        [Column("reply_to_id")]
        public long? ReplyToId { get; set; }

        [Column("archivo_url")]
        public string? ArchivoUrl { get; set; }

        [Column("archivo_nombre_original")]
        public string? ArchivoNombreOriginal { get; set; }
    }
}
