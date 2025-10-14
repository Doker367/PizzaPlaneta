using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ManyBoxApi.Models
{
    [Table("envios")]
    public class Envio
    {
        public int Id { get; set; }

        [Column("guia_rastreo")]
        public string GuiaRastreo { get; set; } = string.Empty;

        [Column("fecha_entrega")]
        public DateTime? FechaEntrega { get; set; }

        [Column("empleado_id")]
        public int? EmpleadoId { get; set; }
        public Empleado Empleado { get; set; }

        [Column("venta_id")]
        public int VentaId { get; set; }
        public Venta Venta { get; set; }

        // Relación con SeguimientoPaquete (por EnvioId)
        public ICollection<SeguimientoPaquete> Seguimientos { get; set; } = new List<SeguimientoPaquete>();

    }
}
