using System;
using System.Collections.Generic;

namespace ManyBoxApi.DTOs
{
    public class MovimientoSeguimientoDto
    {
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }

    public class SeguimientoPaqueteDto
    {
        public string Id { get; set; } = string.Empty; // Número de guía
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaActualizacion { get; set; }
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public List<MovimientoSeguimientoDto> Historial { get; set; } = new();
    }
}
