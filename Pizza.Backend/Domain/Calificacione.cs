using System;
using System.Collections.Generic;

namespace Pizza.Backend.Domain;

public partial class Calificacione
{
    public int Id { get; set; }

    public int PedidoId { get; set; }

    public int UsuarioId { get; set; }

    public int SucursalId { get; set; }

    public int? Puntuacion { get; set; }

    public string? Comentario { get; set; }

    public DateTime? Fecha { get; set; }

    public virtual Pedido Pedido { get; set; } = null!;

    public virtual Sucursale Sucursal { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
