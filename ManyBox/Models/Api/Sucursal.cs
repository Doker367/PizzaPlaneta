namespace ManyBox.Models.Api
{
    public class Sucursal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty; // corresponde a columna 'sucursaldireccion'
    }
}
