using System;

namespace ManyBoxApi.DTOs
{
    public class CrearEnvioRequest
    {
        public int VentaId { get; set; } // Solo se necesita la venta relacionada
        public DateTime? FechaEntrega { get; set; } // Si se quiere registrar la fecha de entrega desde el front
    }
}