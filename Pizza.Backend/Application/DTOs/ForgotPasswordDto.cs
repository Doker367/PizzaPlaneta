using System.ComponentModel.DataAnnotations;

namespace Pizza.Backend.Application.DTOs
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
