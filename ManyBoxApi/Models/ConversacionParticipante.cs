using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManyBoxApi.Models
{
    [Table("conversacionparticipantes")]
    public class ConversacionParticipante
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("conversacion_id")]
        public int ConversacionId { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("rol")]
        public string? Rol { get; set; }

        [Column("fecha_union")]
        public DateTime? FechaUnion { get; set; }
    }
}
