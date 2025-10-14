namespace ManyBoxApi.DTOs
{
    public class EnvioDto
    {
        public int Id { get; set; }
        public DateTime? FechaEntrega { get; set; } // Fecha de entrega real, si aplica
        public int VentaId { get; set; } // Relación con la venta
    }
}
