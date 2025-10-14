namespace ManyBox.Models
{
    public class MensajeLeidoDTO
    {
        public long Id { get; set; }
        public long MensajeId { get; set; }
        public int UsuarioId { get; set; }
        public string Estado { get; set; } = "enviado"; // "enviado", "recibido", "leido"
        public DateTime FechaEstado { get; set; }
    }
}