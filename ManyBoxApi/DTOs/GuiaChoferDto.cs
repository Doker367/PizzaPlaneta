using System;
using System.Collections.Generic;

namespace ManyBoxApi.DTOs
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
}
