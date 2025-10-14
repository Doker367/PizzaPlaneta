using System;
using System.Collections.Generic;

namespace ManyBoxApi.DTOs
{
    public class CrearVentaRequest
    {
        public DateTime Fecha { get; set; }
        public int? Cliente_Id { get; set; }
        public int? Empleado_Id { get; set; }
        public string Folio { get; set; }
        public string Remitente_Nombre { get; set; }
        public string Remitente_Telefono { get; set; }
        public string? Remitente_Compania { get; set; } // ahora opcional
        public string Remitente_Direccion { get; set; }
        public string Remitente_Ciudad { get; set; }
        public string Remitente_Estado { get; set; }
        public string Remitente_Pais { get; set; }
        public string Remitente_Cp { get; set; }
        public string Destinatario_Nombre { get; set; }
        public string Destinatario_Telefono { get; set; }
        public string? Destinatario_Compania { get; set; } // ahora opcional
        public string Destinatario_Direccion { get; set; }
        public string Destinatario_Ciudad { get; set; }
        public string Destinatario_Estado { get; set; }
        public string Destinatario_Pais { get; set; }
        public string Destinatario_Cp { get; set; }
        public decimal? Valor_Declarado { get; set; }
        public string Medidas { get; set; }
        public decimal? Peso_Volumetrico { get; set; }
        public decimal? Peso_Fisico { get; set; }
        public bool? Seguro { get; set; }
        public string Compania_Envio { get; set; }
        public string Tipo_Riesgo { get; set; }
        public string Tipo_Pago { get; set; }
        public decimal? Costo_Envio { get; set; }
        public int? Total_Piezas { get; set; }
        public string Tiempo_Estimado { get; set; }
        public decimal? Total_Cobrado { get; set; }
        public List<DetalleContenidoRequest> DetalleContenido { get; set; } = new();
        // Nueva bandera para permitir ventas incompletas (false) y terminadas (true)
        public bool Completada { get; set; }
    }

    public class DetalleContenidoRequest
    {
        public string Descripcion { get; set; }
        public decimal Cantidad { get; set; }
        public string? Unidad { get; set; }
    }
}