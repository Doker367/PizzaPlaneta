
using System.Collections.Generic;
using System.Linq;

namespace Pizza.Backend.Application.DTOs
{
    public class CarritoDto
    {
        public List<CarritoItemDto> Items { get; set; } = new List<CarritoItemDto>();
        public decimal Total => Items.Sum(item => item.Subtotal);
    }
}
