using System.ComponentModel.DataAnnotations.Schema;

namespace ManyBoxApi.Models
{
    [Table("notificaciondestinatarios")]
    public class NotificacionDestinatario
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("notificacion_id")]
        public int NotificacionId { get; set; }
        public Notificacion Notificacion { get; set; }

        [Column("usuario_id")]
        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        [Column("sucursal_id")]
        public int? SucursalId { get; set; }
        public Sucursal? Sucursal { get; set; }

        [Column("rol_id")]
        public int? RolId { get; set; }
        public Rol? Rol { get; set; }

        [Column("tipo_destinatario")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string? TipoDestinatario { get; set; }
    }
}