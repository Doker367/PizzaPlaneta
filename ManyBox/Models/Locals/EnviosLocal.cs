using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyBox.Models.Locals
{
    [Table("Envios")]
    public class EnvioLocal
    {
        [PrimaryKey]
        public Guid IdEnvio { get; set; } = Guid.NewGuid();

        [Indexed]
        public Guid IdRemitenteAsociado { get; set; }

        [Indexed]
        public Guid IdDestinatarioAsociado { get; set; }

        public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;
    }
}
