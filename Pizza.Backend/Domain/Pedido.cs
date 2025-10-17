using System;
using System.Collections.Generic;

namespace Pizza.Backend.Domain;

public partial class Pedido
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public int SucursalId { get; set; }

    public DateTime? Fecha { get; set; }

    public string Estado { get; set; } = null!;

    public decimal Total { get; set; }

    public int? TarjetaId { get; set; }

    public virtual ICollection<Calificacione> Calificaciones { get; set; } = new List<Calificacione>();

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    public virtual ICollection<HistorialEstadoPedido> HistorialEstadoPedidos { get; set; } = new List<HistorialEstadoPedido>();

    public virtual Sucursale Sucursal { get; set; } = null!;

    public virtual Tarjeta? Tarjeta { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
