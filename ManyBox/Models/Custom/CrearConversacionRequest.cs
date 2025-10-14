using System.Collections.Generic;

namespace ManyBox.Models.Custom
{
    public class CrearConversacionRequest
    {
        public List<int> ParticipantesIds { get; set; } = new();
        // Solo acepta 'directo' o 'grupo' para coincidir con el ENUM de la base de datos
        public string Tipo { get; set; } = "grupo";
        public string? Nombre { get; set; }
    }
}