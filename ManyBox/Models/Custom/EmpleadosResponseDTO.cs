using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyBox.Models.Custom
{
    public class EmpleadoDTO
    {
        public Guid IdEmpleadoAsociado { get; set; }
        public Guid IdEmpleado { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Rol { get; set; }
        public string? Direccion { get; set; }
        public string? CURP { get; set; }
        public string? Genero { get; set; }
        public DateTime FechaIngreso { get; set; }
        public bool Estatus { get; set; }
    }
}
