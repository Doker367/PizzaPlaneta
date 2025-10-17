
using System.ComponentModel.DataAnnotations;

namespace Pizza.Backend.Application.DTOs
{
    public class AddTarjetaDto
    {
        [Required]
        [StringLength(100)]
        public string NombreTarjeta { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string NumeroEnmascarado { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Marca { get; set; } = string.Empty;

        [Required]
        [StringLength(7)] // MM/YY
        public string FechaVencimiento { get; set; } = string.Empty;

        [Required]
        public string TokenPago { get; set; } = string.Empty;
    }
}
