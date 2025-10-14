namespace ManyBox.Models
{
    public class GuiaChoferDto
    {
        public int EnvioId { get; set; }
        public string GuiaRastreo { get; set; } = string.Empty;
        public string Destinatario { get; set; } = string.Empty;
        public int VentaId { get; set; }
        public List<EstadoGuiaDto> Estados { get; set; } = new();
        public int EstadoActual { get; set; }
        public bool Expandido { get; set; }
    }

    public class EstadoGuiaDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
    }

    public class NotificacionDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public bool Leido { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class Entrega
    {
        public string Guia { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Destinatario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
