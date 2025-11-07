namespace Pizza.Backend.Application.DTOs;

public class ProductItemDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal PrecioUnitario { get; set; }
    public int? Calorias { get; set; }
    public string? Categoria { get; set; }
    public int Cantidad { get; set; }
}

