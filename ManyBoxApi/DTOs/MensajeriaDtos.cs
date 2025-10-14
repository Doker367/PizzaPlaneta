namespace ManyBoxApi.DTOs
{
    public class CrearConversacionRequest
    {
        public string Tipo { get; set; } = "directa"; // directa | grupo
        public string? Nombre { get; set; }
        public List<int> ParticipantesIds { get; set; } = new();
    }
}
