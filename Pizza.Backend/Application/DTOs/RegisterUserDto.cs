using System.ComponentModel.DataAnnotations;

namespace Pizza.Backend.Application.DTOs;

public class RegisterUserDto
{
    [Required]
    public string Nombre { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public string? Telefono { get; set; }

    [Required]
    public string Password { get; set; } = null!;
}
