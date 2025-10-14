using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManyBoxApi.Models
{
    [Table("detallecontenido")]
    public class DetalleContenido
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("venta_id")]
        public int VentaId { get; set; }

        [Column("descripcion")]
        public required string Descripcion { get; set; }

        [Column("cantidad")]
        public decimal Cantidad { get; set; }

        [Column("unidad")]
        public string? Unidad { get; set; }
    }
}