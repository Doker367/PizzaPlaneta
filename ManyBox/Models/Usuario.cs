namespace ManyBox.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int SucursalId { get; set; }
        public int RolId { get; set; }
        public string RolNombre { get; set; }
    }
}