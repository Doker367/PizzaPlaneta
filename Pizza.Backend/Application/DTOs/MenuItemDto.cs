namespace Pizza.Backend.Application.DTOs
{
    public class MenuItemDto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int? Calorias { get; set; }
        public bool Disponible { get; set; }
    }
}
