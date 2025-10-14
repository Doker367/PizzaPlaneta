using System.ComponentModel.DataAnnotations;

namespace ManyBoxApi.ViewModels
{
    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "La nueva contraseña es requerida.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La nueva contraseña debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,100}$",
            ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula, un número y un carácter especial.")]
        public required string NewPassword { get; set; } // Agrega 'required'

        [Required(ErrorMessage = "La confirmación de la nueva contraseña es requerida.")]
        [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la confirmación no coinciden.")]
        public required string ConfirmNewPassword { get; set; } // Agrega 'required'
    }
}