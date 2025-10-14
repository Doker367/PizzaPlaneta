using System;

namespace ManyBoxApi.Models
{
    public class RemitenteSyncDTO
    {
        public Guid IdRemitenteAsociado { get; set; }
        public Guid IdRemitente { get; set; }
        public string? Nombre { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Estado { get; set; }
        public string? Pais { get; set; }
        public string? CP { get; set; }
        public string? Celular { get; set; }
    }
}
