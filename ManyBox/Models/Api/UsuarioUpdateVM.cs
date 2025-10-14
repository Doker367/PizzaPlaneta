using System.ComponentModel.DataAnnotations;

namespace ManyBox.Models.Api
{
    public class UsuarioUpdateVM
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es requerido.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El email es requerido.")]
        [EmailAddress(ErrorMessage = "El email no es válido.")]
        public string Email { get; set; }

        public int? RolId { get; set; }
        public bool? Activo { get; set; }

        // Nuevos campos para actualizar empleado
        public string? Telefono { get; set; }
        public DateTime? FechaNacimiento { get; set; }
    }
}
