using System.ComponentModel.DataAnnotations.Schema;

namespace ManyBoxApi.Models
{
    [Table("destinatarios")]
    public class Destinatario
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public required string Nombre { get; set; } = null!;

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