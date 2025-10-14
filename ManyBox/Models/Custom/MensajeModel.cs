using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyBox.Models.Custom
{
    public class MensajeModel
    {
        public long Id { get; set; }
        public int ConversacionId { get; set; }
        public int UsuarioId { get; set; }
        public string Contenido { get; set; }
        public string Tipo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Editado { get; set; }
        public bool Eliminado { get; set; }
        public bool LeidoPorMi { get; set; }
        public long? ReplyToId { get; set; }
        public string? UsuarioNombre { get; set; }
        public string? ArchivoUrl { get; set; }
        public string? ArchivoNombreOriginal { get; set; }
    }
}