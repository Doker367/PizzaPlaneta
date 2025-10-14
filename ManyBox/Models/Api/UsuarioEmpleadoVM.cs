using System;

namespace ManyBox.Models.Api
{
    public class UsuarioEmpleadoVM
    {
        public int UsuarioId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioApellido { get; set; } = string.Empty;
        public int? EmpleadoId { get; set; }
        public string? EmpleadoNombre { get; set; }
        public string? EmpleadoApellido { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? CorreoConcatenado { get; set; }
        public string? Telefono { get; set; }
        public string? SucursalNombre { get; set; }
        public string? NombreRol { get; set; }
    }
}
