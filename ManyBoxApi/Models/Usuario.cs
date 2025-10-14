using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ManyBoxApi.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string PasswordHash { get; set; }
        [Column("rol_id")]
        public int RolId { get; set; }
        public Rol? Rol { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
        [Column("empleado_id")]
        public int? EmpleadoId { get; set; }

        // Propiedad de navegación para Empleado
        public virtual Empleado? Empleado { get; set; }

        // Relaciones
        public ICollection<NotificacionDestinatario> NotificacionesRecibidas { get; set; } = new List<NotificacionDestinatario>();
        public ICollection<NotificacionEntrega> Entregas { get; set; } = new List<NotificacionEntrega>();
    }
}