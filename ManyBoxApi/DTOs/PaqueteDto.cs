using System;
using System.Collections.Generic;

namespace ManyBoxApi.DTOs
{
    public class PaqueteDto
    {
        public int Id { get; set; }
        public int VentaId { get; set; } // <-- AGREGADO para exponer el id de venta
        public string CodigoSeguimiento { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string? SucursalOrigen { get; set; }
        public string RemitenteNombre { get; set; }
        public string RemitenteTelefono { get; set; }
        public string DestinatarioNombre { get; set; }
        public string DestinatarioTelefono { get; set; }
        public string DireccionDestino { get; set; }
        public string CiudadDestino { get; set; }
        public string TipoEnvio { get; set; }
        public string EstadoActual { get; set; }
        public DateTime? FechaUltimaActualizacion { get; set; }
        public string? EmpleadoRegistro { get; set; }
        public string EmpleadoActual { get; set; }
        public string RepartidorAsignado { get; set; }
        public decimal PesoKg { get; set; }
        public string NumeroGuia { get; set; }
        public string PaquetesAsignados { get; set; }
        public decimal CostoEnvio { get; set; }
    }
}
