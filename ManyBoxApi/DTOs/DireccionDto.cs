namespace ManyBoxApi.DTOs
{
    public class DireccionDto
    {
        public int Id { get; set; }
        public int? ClienteId { get; set; }
        public string DireccionTexto { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string CodigoPostal { get; set; } = string.Empty;
    }
}
