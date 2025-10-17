using System;
using System.Collections.Generic;

namespace Pizza.Backend.Domain;

public partial class Sucursale
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Ciudad { get; set; } = null!;

    public string? Estado { get; set; }

    public string? Telefono { get; set; }

    public string GoogleMapsUrl { get; set; } = null!;

    public virtual ICollection<Calificacione> Calificaciones { get; set; } = new List<Calificacione>();

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
