using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManyBoxApi.Models
{
    [Table("ventas")]   
    public class Venta
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Column("cliente_id")]
        public int? Cliente_Id { get; set; }
        [Column("empleado_id")]
        public int? Empleado_Id { get; set; }

        [Column("folio")]
        [MaxLength(15)]
        public string? Folio { get; set; }

        [Column("valor_declarado")]
        public decimal? Valor_Declarado { get; set; }

        [Column("medidas")]
        public string? Medidas { get; set; }

        [Column("peso_volumetrico")]
        public decimal? Peso_Volumetrico { get; set; }

        [Column("peso_fisico")]
        public decimal? Peso_Fisico { get; set; }

        [Column("seguro")]
        public bool? Seguro { get; set; }

        [Column("compania_envio")]
        public string? Compania_Envio { get; set; }

        [Column("tipo_riesgo")]
        public string? Tipo_Riesgo { get; set; }

        [Column("tipo_pago")]
        public string? Tipo_Pago { get; set; }

        [Column("costo_envio")]
        public decimal? Costo_Envio { get; set; }

        [Column("total_piezas")]
        public int? Total_Piezas { get; set; }

        [Column("tiempo_estimado")]
        public string? Tiempo_Estimado { get; set; }

        [Column("total_cobrado")]
        public decimal? Total_Cobrado { get; set; }

        [Column("remitente_id")]
        public int? Remitente_Id { get; set; }

        [Column("destinatario_id")]
        public int? Destinatario_Id { get; set; }

        [Column("completada")]
        public bool Completada { get; set; }

        // Relaciones de navegación (asumiendo que tienes estas clases definidas)
        public Remitente Remitente { get; set; }
        public Destinatario Destinatario { get; set; }
        public ICollection<DetalleVenta> DetalleVentas { get; set; }
        public ICollection<DetalleContenido> DetalleContenido { get; set; }
        public ICollection<Envio> Envios { get; set; }

    }
}