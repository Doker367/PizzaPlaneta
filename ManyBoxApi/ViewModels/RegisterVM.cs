using System.ComponentModel.DataAnnotations;

namespace ManyBoxApi.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public required string Nombre { get; set; } // <-- ¡Aquí el cambio!

        [Required(ErrorMessage = "El email es requerido.")]
        [EmailAddress(ErrorMessage = "El email no es una dirección de correo electrónico válida.")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres.")]
        public required string Email { get; set; } // <-- ¡Aquí el cambio!

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,100}$",
            ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula, un número y un carácter especial.")]
        public required string Password { get; set; } // <-- ¡Aquí el cambio!

        [Required(ErrorMessage = "La confirmación de la contraseña es requerida.")]
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
        public required string ConfirmPassword { get; set; } // <-- ¡Aquí el cambio!

        // Este RolId NO se usará para el registro público (Auth/register)
        // pero es útil para Auth/admin-register
        public int? RolId { get; set; } // Este ya es nullable (int?), por lo que no necesita 'required'
    }
}