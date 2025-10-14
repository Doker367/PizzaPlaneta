namespace ManyBoxApi.DTOs
{
    public class EnviarMensajeRequest
    {
        public string Contenido { get; set; } = string.Empty;
        public string Tipo { get; set; } = "texto";
        public long? ReplyToId { get; set; }
        public string? ArchivoUrl { get; set; }
        public string? ArchivoNombreOriginal { get; set; }
    }
}
