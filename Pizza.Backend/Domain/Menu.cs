namespace Pizza.Backend.Domain;

public class Menu
{
    public int Id { get; set; }
    public int SucursalId { get; set; }
    public int ProductoId { get; set; }
    public decimal? PrecioEspecial { get; set; }
    public bool Disponible { get; set; } = true;

    public virtual Sucursale Sucursal { get; set; }
}
