namespace ManyBoxApi.Models
{
    public class DetalleVenta
    {
        public int VentaId { get; set; }
        public int ProductoId { get; set; }

        // Propiedades de navegación obligatorias para EF Core
        public Venta Venta { get; set; } = null!;
        public Producto Producto { get; set; } = null!;
    }
}