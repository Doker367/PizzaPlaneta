using System.ComponentModel.DataAnnotations;

namespace ManyBoxApi.DTOs
{
    public class ConversacionDTO
    {
        public int Id { get; set; }

        [Required]
        public string Tipo { get; set; } = "directa";

        [StringLength(100)]
        public string? Nombre { get; set; }

        [Required]
        public int CreadoPor { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Lista de IDs de participantes
        public List<int> ParticipantesIds { get; set; } = new List<int>();

        // Información extendida de participantes si la necesitas
        public List<ParticipanteInfoDTO>? Participantes { get; set; }
    }

    public class ParticipanteInfoDTO
    {
        public int UsuarioId { get; set; }
        public string? Rol { get; set; }
        public DateTime? FechaUnion { get; set; }
    }
}