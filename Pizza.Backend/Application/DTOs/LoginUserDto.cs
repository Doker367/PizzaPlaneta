using System.ComponentModel.DataAnnotations;

namespace Pizza.Backend.Application.DTOs;

public class LoginUserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
