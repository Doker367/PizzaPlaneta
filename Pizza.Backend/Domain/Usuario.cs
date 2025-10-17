using System;
using System.Collections.Generic;

namespace Pizza.Backend.Domain;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telefono { get; set; }

    public string PasswordHash { get; set; } = null!;

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<Calificacione> Calificaciones { get; set; } = new List<Calificacione>();

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    public virtual ICollection<Tarjeta> Tarjetas { get; set; } = new List<Tarjeta>();

    public virtual Carrito? Carrito { get; set; }
}
