
namespace Pizza.Backend.Application.DTOs
{
    public class TarjetaDto
    {
        public int Id { get; set; }
        public string NombreTarjeta { get; set; } = string.Empty;
        public string NumeroEnmascarado { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string? FechaVencimiento { get; set; }
    }
}
