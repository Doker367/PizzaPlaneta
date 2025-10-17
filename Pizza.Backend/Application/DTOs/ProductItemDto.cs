namespace Pizza.Backend.Application.DTOs;

public class ProductItemDto
{
    public string Nombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
