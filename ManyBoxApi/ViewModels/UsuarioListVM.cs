namespace ManyBoxApi.ViewModels
{
    public class UsuarioListVM
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string RolNombre { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}