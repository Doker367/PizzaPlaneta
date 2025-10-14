using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ManyBoxApi.Models;
using System.Linq;

namespace ManyBoxApi.Models
{
    [Table("notificaciones")]
    public class Notificacion
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("tipo")]
        public string Tipo { get; set; }

        [Column("titulo")]
        public string Titulo { get; set; }

        [Column("mensaje")]
        public string Mensaje { get; set; }

        [Column("prioridad")]
        public string Prioridad { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [Column("datos")]
        public string? Datos { get; set; }

        [Column("remitente_id")]
        public int? RemitenteId { get; set; }

        [Column("tipo_objeto")]
        public string? TipoObjeto { get; set; }

        [Column("objeto_id")]
        public int? ObjetoId { get; set; }

        [Column("estado")]
        public string? Estado { get; set; }

        [Column("idioma")]
        public string? Idioma { get; set; }

        [Column("expiracion")]
        public DateTime? Expiracion { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        // Relaciones
        public ICollection<NotificacionEntrega> Entregas { get; set; } = new List<NotificacionEntrega>();
        public ICollection<NotificacionDestinatario> Destinatarios { get; set; } = new List<NotificacionDestinatario>();

        // Indica si la notificación fue leída por todos los destinatarios
        [NotMapped]
        public bool Leido {
            get {
                if (Entregas == null || Entregas.Count == 0) return false;
                return Entregas.All(e => e.Estado == "leida");
            }
        }
    }
}