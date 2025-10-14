using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManyBoxApi.Models
{
    [Table("roles")]
    public class Rol
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("permisos")]
        public string? Permisos { get; set; }

        // Navegación inversa
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
