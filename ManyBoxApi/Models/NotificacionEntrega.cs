using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManyBoxApi.Models
{
    [Table("notificacionentregas")]
    public class NotificacionEntrega
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("notificacion_id")]
        public int NotificacionId { get; set; }
        public Notificacion Notificacion { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [Column("canal")]
        public string Canal { get; set; } // Ejemplo: "ws", "push", "email", "sms"

        [Column("estado")]
        public string Estado { get; set; } // Ejemplo: "pendiente", "enviada", "fallida", "leida"

        [Column("intentos_envio")]
        public int IntentosEnvio { get; set; }

        [Column("fecha_envio")]
        public DateTime? FechaEnvio { get; set; }

        [Column("fecha_leido")]
        public DateTime? FechaLeido { get; set; }

        [Column("device_id")]
        public string? DeviceId { get; set; }

        [Column("error")]
        public string? Error { get; set; }

        [Column("ultimo_error")]
        public string? UltimoError { get; set; }
    }
}