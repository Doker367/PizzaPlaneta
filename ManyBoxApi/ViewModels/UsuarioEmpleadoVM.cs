namespace ManyBoxApi.ViewModels
{
    public class UsuarioEmpleadoVM
    {
        public int UsuarioId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioApellido { get; set; } = string.Empty;
        public int? EmpleadoId { get; set; }
        public string EmpleadoNombre { get; set; } = string.Empty;
        public string EmpleadoApellido { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }
        public string CorreoConcatenado { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string SucursalNombre { get; set; } = string.Empty;
        public string NombreRol { get; set; } = string.Empty;
    }
}