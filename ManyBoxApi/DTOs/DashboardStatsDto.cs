namespace ManyBoxApi.DTOs
{
    public class DashboardStatsDto
    {
        public int EnviosActivos { get; set; }
        public int TasaEntrega { get; set; } // Porcentaje (0-100)
        public int Satisfaccion { get; set; } // Porcentaje (0-100)
        public int NuevosPaquetes { get; set; } // Nuevo campo
    }
}
