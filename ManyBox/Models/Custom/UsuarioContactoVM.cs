namespace ManyBox.Models.Custom
{
    public class UsuarioContactoVM
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public required string SucursalNombre { get; set; }
        public int? SucursalId { get; set; } // <-- necesario para notificaciones
        public bool Selected { get; set; }
    }
}