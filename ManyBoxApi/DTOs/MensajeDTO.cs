namespace ManyBox.Models
{
    public class MensajeDTO
    {
        public long Id { get; set; }
        public int ConversacionId { get; set; }
        public int UsuarioId { get; set; }
        public string Contenido { get; set; } = string.Empty;
        public string Tipo { get; set; } = "texto"; // "texto", "imagen", "archivo"
        public DateTime FechaCreacion { get; set; }
        public bool Editado { get; set; }
        public bool Eliminado { get; set; }
        public bool LeidoPorMi { get; set; }
        public long? ReplyToId { get; set; } // Nuevo campo para soporte de mensajes respondidos

    }
}