using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyBox.Models.Locals
{
    [Table("Destinatarios")]
    public class DestinatarioLocal
    {
        [PrimaryKey]
        [Column("IdDestinatarioAsociado")]
        public Guid IdDestinatarioAsociado { get; set; }

        [Column("IdDestinatario")]
        public Guid IdDestinatario { get; set; }

        [Column("Nombre")]
        public string? Nombre { get; set; }

        [Column("Direccion")]
        public string? Direccion { get; set; }

        [Column("Ciudad")]
        public string? Ciudad { get; set; }

        [Column("Estado")]
        public string? Estado { get; set; }

        [Column("Pais")]
        public string? Pais { get; set; }

        [Column("CP")]
        public string? CP { get; set; }

        [Column("Celular")]
        public string? Celular { get; set; }
    }
}
