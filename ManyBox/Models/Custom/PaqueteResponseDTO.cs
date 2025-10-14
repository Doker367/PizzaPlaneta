using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyBox.Models.Custom
{
    public class PaqueteResponseDTO
    {
        public Guid IdPaqueteAsociado { get; set; }
        public Guid IdPaquete { get; set; }

        // Referencias a remitente y destinatario
        public Guid IdRemitenteAsociado { get; set; }
        public Guid IdDestinatarioAsociado { get; set; }

        // Propiedades del paquete
        public double PesoKg { get; set; }
        public double VolumenM3 { get; set; }
        public string? DescripcionContenido { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
