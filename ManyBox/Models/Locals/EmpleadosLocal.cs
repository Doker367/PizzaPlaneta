using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyBox.Models.Locals
{
    [Table("Empleados")]
    public class EmpleadoLocal
    {
        [PrimaryKey]
        [Column("IdEmpleadoAsociado")]
        public Guid IdEmpleadoAsociado { get; set; } = Guid.NewGuid();

        [Column("IdEmpleado")]
        public Guid IdEmpleado { get; set; } = Guid.NewGuid();

        [Column("Nombre")]
        public string? Nombre { get; set; }

        [Column("Correo")]
        public string? Correo { get; set; }

        [Column("Telefono")]
        public string? Telefono { get; set; }

        [Column("Rol")]
        public string? Rol { get; set; }  // Ej: "Repartidor", "Administrador", "Atención a Clientes"

        [Column("Direccion")]
        public string? Direccion { get; set; }

        [Column("CURP")]
        [MaxLength(18)]
        public string? CURP { get; set; }

        [Column("Genero")]
        public string? Genero { get; set; }  // Ej: "Masculino", "Femenino", "No binario"

        [Column("FechaIngreso")]
        public DateTime FechaIngreso { get; set; } = DateTime.UtcNow;

        [Column("Estatus")]
        public bool Estatus { get; set; } = true;  // true = Activo, false = Inactivo
    }
}
