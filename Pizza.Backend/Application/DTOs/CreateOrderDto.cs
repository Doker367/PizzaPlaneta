using System.ComponentModel.DataAnnotations;

namespace Pizza.Backend.Application.DTOs;

public class CreateOrderDto
{
    [Required]
    public int SucursalId { get; set; }

    [Required]
    [MinLength(1)]
    public List<OrderItemDto> Items { get; set; } = new();
}
