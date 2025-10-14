using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyBox.Models.Custom
{
    public class DestinatarioResponseDTO
    {
        public Guid IdDestinatarioAsociado { get; set; }
        public Guid IdDestinatario { get; set; }
        public string? Nombre { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Estado { get; set; }
        public string? Pais { get; set; }
        public string? CP { get; set; }
        public string? Celular { get; set; }
    }
}
