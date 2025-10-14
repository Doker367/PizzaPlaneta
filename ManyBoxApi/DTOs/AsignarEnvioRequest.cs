namespace ManyBoxApi.DTOs
{
    public class AsignarEnvioRequest
    {
        public string CodigoSeguimiento { get; set; } = string.Empty;
        public int EmpleadoId { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public int VentaId { get; set; }
    }
}
