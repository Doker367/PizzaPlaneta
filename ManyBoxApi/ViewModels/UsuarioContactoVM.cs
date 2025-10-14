namespace ManyBoxApi.ViewModels
{
    public class UsuarioContactoVM
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Nombre { get; set; } = string.Empty;
        public string? Apellido { get; set; } = string.Empty;
        public string? SucursalNombre { get; set; } = string.Empty;
    }
}
