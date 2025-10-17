
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pizza.Backend.Domain
{
    [Table("carrito_items")]
    public class CarritoItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("carrito_id")]
        public int CarritoId { get; set; }

        [ForeignKey("CarritoId")]
        public virtual Carrito? Carrito { get; set; }

        [Column("producto_id")]
        public int ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public virtual Producto? Producto { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }
    }
}
