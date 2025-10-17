using System;
using System.Collections.Generic;

namespace Pizza.Backend.Domain;

public partial class HistorialEstadoPedido
{
    public int Id { get; set; }

    public int PedidoId { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime? Fecha { get; set; }

    public virtual Pedido Pedido { get; set; } = null!;
}
