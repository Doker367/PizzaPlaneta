using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ManyBoxApi.Models
{

    [Table("empleados")]
    public class Empleado
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("apellido")]
        public string Apellido { get; set; }

        [Column("fecha_nacimiento", TypeName = "date")]
        public DateTime? FechaNacimiento { get; set; }

        [Column("correo")]
        public string Correo { get; set; }

        [Column("telefono")]
        public string Telefono { get; set; }

        [Column("sucursal_id")]
        public int? SucursalId { get; set; }

        // Propiedad de navegación (opcional, si tienes la entidad Sucursal)
        public virtual Sucursal? Sucursal { get; set; }
        public ICollection<Envio> Envios { get; set; } = new List<Envio>();

    }
}

