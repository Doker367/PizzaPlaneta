namespace ManyBox.Models.Custom
{
    public class UsuarioVM
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string RolNombre { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public int? SucursalId { get; set; } // <-- Agregado para acceso a la sucursal del usuario
    }
}
