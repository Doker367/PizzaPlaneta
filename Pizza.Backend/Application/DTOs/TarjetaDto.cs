
namespace Pizza.Backend.Application.DTOs
{
    public class TarjetaDto
    {
        public int Id { get; set; }
        public string NombreTarjeta { get; set; } = string.Empty;
        public string Last4 { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }
    }
}
