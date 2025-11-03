using System;

namespace Pizza.Backend.Domain
{
    public partial class Favorito
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int ProductoId { get; set; }
        public DateTime FechaAgregado { get; set; }

        public virtual Usuario Usuario { get; set; } = null!;
        // Nota: No hay una propiedad de navegación a Producto para mantener los contextos de DB separados.
    }
}
