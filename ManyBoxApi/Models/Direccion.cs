using System.ComponentModel.DataAnnotations.Schema;

namespace ManyBoxApi.Models
{
    [Table("direcciones")]  
    public class Direccion
    {
        public int Id { get; set; }

        [Column("cliente_id")]
        public int? ClienteId { get; set; }

        [Column("direccion")]
        public string DireccionTexto { get; set; } // Evita conflicto con nombre de clase

        [Column("ciudad")]
        public string Ciudad { get; set; }

        [Column("estado")]
        public string Estado { get; set; }

        [Column("pais")]
        public string Pais { get; set; }

        [Column("codigo_postal")]
        public string CodigoPostal { get; set; }
    }
}