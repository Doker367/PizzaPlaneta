using System.ComponentModel.DataAnnotations;

namespace Pizza.Backend.Application.DTOs;

public class CreateOrderDto
{
    [Required]
    public int SucursalId { get; set; }

    [Required]
    [MinLength(1)]
    public List<OrderItemDto> Items { get; set; } = new();

    [Required]
    public string MetodoPago { get; set; } = string.Empty;

    public int? TarjetaId { get; set; }
}
