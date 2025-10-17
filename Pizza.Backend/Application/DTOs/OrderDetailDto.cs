namespace Pizza.Backend.Application.DTOs;

public class OrderDetailDto
{
    public int PedidoId { get; set; }
    public string Sucursal { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<ProductItemDto> Productos { get; set; } = new();
    public int? Calificacion { get; set; }
    public string? Comentario { get; set; }
}
