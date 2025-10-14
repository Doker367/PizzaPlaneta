using System.ComponentModel.DataAnnotations;

namespace ManyBoxApi.DTOs
{
    public class CrearConversacionDTO
    {
        [Required]
        public string Tipo { get; set; } = "grupo"; // "grupo" o "privado"

        [StringLength(100)]
        public string? Nombre { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Debe haber al menos un participante")]
        public List<int> ParticipantesIds { get; set; } = new List<int>();
    }
}