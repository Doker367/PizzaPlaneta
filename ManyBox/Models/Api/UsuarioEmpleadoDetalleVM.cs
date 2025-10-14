using System;

namespace ManyBox.Models.Api
{
    public class UsuarioEmpleadoDetalleVM
    {
        public int UsuarioId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioApellido { get; set; } = string.Empty;
        public string NombreRol { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public int? EmpleadoId { get; set; }
        public string? EmpleadoNombre { get; set; }
        public string? EmpleadoApellido { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public int? SucursalId { get; set; }
        public string? SucursalNombre { get; set; }
    }
}