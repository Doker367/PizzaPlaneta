using System;
namespace ManyBoxApi.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public ICollection<DetalleVenta> DetalleProducto { get; set; }
    }
}