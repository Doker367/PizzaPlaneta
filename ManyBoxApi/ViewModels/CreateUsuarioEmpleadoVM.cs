using System;
using System.ComponentModel.DataAnnotations;

namespace ManyBoxApi.ViewModels
{
    public class CreateUsuarioEmpleadoVM
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,100}$",
            ErrorMessage = "La contrase�a debe tener al menos 8 caracteres, una may�scula, una min�scula, un n�mero y un car�cter especial.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public int RolId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [Phone]
        public string Telefono { get; set; } = string.Empty;

        [Required]
        public int SucursalId { get; set; }

        [StringLength(100)]
        public string? SucursalNombre { get; set; }
    }
}
