using System.Collections.Generic;

namespace Pizza.Backend.Application.DTOs
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Telefono { get; set; }
        public List<TarjetaDto> Tarjetas { get; set; } = new List<TarjetaDto>();
        public List<OrderDetailDto> HistorialPedidos { get; set; } = new List<OrderDetailDto>();
    }
}
