using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManyBoxApi.DTOs
{
    public class VentaDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        [JsonPropertyName("folio")]
        public string? Folio { get; set; }
        [JsonPropertyName("valor_Declarado")]
        public decimal? Valor_Declarado { get; set; }
        [JsonPropertyName("medidas")]
        public string? Medidas { get; set; }
        [JsonPropertyName("peso_Volumetrico")]
        public decimal? Peso_Volumetrico { get; set; }
        [JsonPropertyName("peso_Fisico")]
        public decimal? Peso_Fisico { get; set; }
        [JsonPropertyName("seguro")]
        public bool? Seguro { get; set; }
        [JsonPropertyName("compania_Envio")]
        public string? Compania_Envio { get; set; }
        [JsonPropertyName("tipo_Riesgo")]
        public string? Tipo_Riesgo { get; set; }
        [JsonPropertyName("tipo_Pago")]
        public string? Tipo_Pago { get; set; }
        [JsonPropertyName("costo_Envio")]
        public decimal? Costo_Envio { get; set; }
        [JsonPropertyName("total_Piezas")]
        public int? Total_Piezas { get; set; }
        [JsonPropertyName("tiempo_Estimado")]
        public string? Tiempo_Estimado { get; set; }
        [JsonPropertyName("total_Cobrado")]
        public decimal? Total_Cobrado { get; set; }
        [JsonPropertyName("remitente")]
        public RemitenteDto? Remitente { get; set; }
        [JsonPropertyName("destinatario")]
        public DestinatarioDto? Destinatario { get; set; }
        [JsonPropertyName("detalleContenido")]
        public List<DetalleContenidoDto>? DetalleContenido { get; set; }
        [JsonPropertyName("empleado")]
        public EmpleadoDto? Empleado { get; set; }
        [JsonPropertyName("sucursalOrigen")]
        public string? SucursalOrigen { get; set; } // Nombre de la sucursal
        [JsonPropertyName("destinatarioNombre")]
        public string? DestinatarioNombre { get; set; } // <-- Para compatibilidad frontend
        public bool Completada { get; set; } // Nueva propiedad
        // Nuevos campos para auto-selección de cliente en el front
        [JsonPropertyName("clienteId")]
        public int? ClienteId { get; set; }
        [JsonPropertyName("clienteNombre")]
        public string? ClienteNombre { get; set; }
    }

    public class RemitenteDto
    {
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
        public string? Compania { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Estado { get; set; }
        public string? Pais { get; set; }
        public string? CP { get; set; }
    }
    public class DestinatarioDto
    {
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
        public string? Compania { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Estado { get; set; }
        public string? Pais { get; set; }
        public string? CP { get; set; }
    }
    public class DetalleContenidoDto
    {
        public string? Descripcion { get; set; }
        public decimal Cantidad { get; set; }
        public string? Unidad { get; set; }
    }
    public class EmpleadoDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
    }
}
