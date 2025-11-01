namespace Pizza.Backend.Application.DTOs;

public class MenuDto
{
    public int Id { get; set; }
    public int SucursalId { get; set; }
    public int ProductoId { get; set; }
    public decimal? PrecioEspecial { get; set; }
    public bool Disponible { get; set; }
}
