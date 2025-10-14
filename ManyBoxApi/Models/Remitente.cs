using System.ComponentModel.DataAnnotations.Schema;

namespace ManyBoxApi.Models
{
    [Table("remitentes")]
    public class Remitente
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("compania")]
        public string? Compania { get; set; }

        [Column("direccion")]
        public string? Direccion { get; set; }

        [Column("ciudad")]
        public string? Ciudad { get; set; }

        [Column("estado")]
        public string? Estado { get; set; }

        [Column("pais")]
        public string? Pais { get; set; }

        [Column("cp")]
        public string? CP { get; set; }
    }
}