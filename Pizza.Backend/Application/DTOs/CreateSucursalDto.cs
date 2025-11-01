namespace Pizza.Backend.Application.DTOs
{
    public class CreateSucursalDto
    {
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string? Estado { get; set; }
        public string? Telefono { get; set; }
        public string GoogleMapsUrl { get; set; }
    }
}
