using System.ComponentModel.DataAnnotations;

namespace ManyBoxApi.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "El usuario es requerido.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida.")]
        public required string Password { get; set; }
    }
}