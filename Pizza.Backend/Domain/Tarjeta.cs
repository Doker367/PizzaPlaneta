using System;
using System.Collections.Generic;

namespace Pizza.Backend.Domain;

public partial class Tarjeta
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string NombreTarjeta { get; set; } = null!;

    public string StripePaymentMethodId { get; set; } = null!;

    public string Last4 { get; set; } = null!;

    public int ExpMonth { get; set; }

    public int ExpYear { get; set; }

    public string Marca { get; set; } = null!;

    public DateTime? FechaGuardado { get; set; }

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    public virtual Usuario Usuario { get; set; } = null!;
}
