namespace ManyBoxApi.DTOs
{
    public class DashboardStatsHistoryDto
    {
        public int EnviosActivos { get; set; }
        public int TasaEntrega { get; set; }
        public int Satisfaccion { get; set; }
        public int NuevosPaquetes { get; set; }
        public DateTime Fecha { get; set; }

    }
}
