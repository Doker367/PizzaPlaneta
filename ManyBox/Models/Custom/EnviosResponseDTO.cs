using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyBox.Models.Custom
{
    public class EnviosResponseDTO
    {
        public Guid IdEnvio { get; set; }
        public Guid IdRemitenteAsociado { get; set; }
        public Guid IdDestinatarioAsociado { get; set; }
        public DateTime FechaEnvio { get; set; }
    }
}
