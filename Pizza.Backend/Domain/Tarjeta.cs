using System;
using System.Collections.Generic;

namespace Pizza.Backend.Domain;

public partial class Tarjeta
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string NombreTarjeta { get; set; } = null!;

    public string NumeroEnmascarado { get; set; } = null!;

    public string? Marca { get; set; }

    public string? FechaVencimiento { get; set; }

    public string? TokenPago { get; set; }

    public DateTime? FechaGuardado { get; set; }

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    public virtual Usuario Usuario { get; set; } = null!;
}
