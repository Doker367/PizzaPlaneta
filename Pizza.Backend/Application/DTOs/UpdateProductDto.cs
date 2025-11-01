namespace Pizza.Backend.Application.DTOs
{
    public class UpdateProductDto
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal? Precio { get; set; }
        public string? Categoria { get; set; }
        public int? Calorias { get; set; }
        public bool? Activo { get; set; }
    }
}
