using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyBox.Models.Custom
{
    public class ConversacionModel
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty; // Valor por defecto para evitar null
        public string? Nombre { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<UsuarioContactoVM> Participantes { get; set; } = new();
        public bool Archivada { get; set; } // NUEVO: para saber si está archivada
    }

    // Extensión para nuevos mensajes y último mensaje
    public class ConversacionModelExt : ConversacionModel
    {
        public int NuevosMensajes { get; set; }
        public DateTime? FechaUltimoMensaje { get; set; }
        public int? UsuarioUltimoMensajeId { get; set; }
    }
}