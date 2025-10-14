using System.ComponentModel.DataAnnotations;

namespace ManyBox.Models.Api
{
    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "La nueva contrase�a es requerida.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La nueva contrase�a debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,100}$",
            ErrorMessage = "La contrase�a debe contener al menos una may�scula, una min�scula, un n�mero y un car�cter especial.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmaci�n de la nueva contrase�a es requerida.")]
        [Compare("NewPassword", ErrorMessage = "La nueva contrase�a y la confirmaci�n no coinciden.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
