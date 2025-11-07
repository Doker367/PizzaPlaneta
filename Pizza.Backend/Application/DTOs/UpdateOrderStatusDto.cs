using System.ComponentModel.DataAnnotations;

namespace Pizza.Backend.Application.DTOs;

public class UpdateOrderStatusDto
{
    [Required]
    public string Status { get; set; }
}
