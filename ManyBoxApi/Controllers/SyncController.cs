using Microsoft.AspNetCore.Mvc;
using ManyBoxApi.Models;
using System.Collections.Generic;

namespace ManyBoxApi.Controllers
{
    [ApiController]
    [Route("api/sync")]
    public class SyncController : ControllerBase
    {
        [HttpPost("remitentes")]
        public IActionResult SyncRemitentes([FromBody] List<RemitenteSyncDTO> remitentes)
        {
            // Aquí mapeas los DTOs a tus entidades y guardas en la base de datos
            // Ejemplo: var entidades = remitentes.Select(dto => new Remitente { ... }).ToList();
            // _dbContext.Remitentes.AddRange(entidades); _dbContext.SaveChanges();
            return Ok(new { success = true, count = remitentes.Count });
        }

        [HttpPost("destinatarios")]
        public IActionResult SyncDestinatarios([FromBody] List<DestinatarioSyncDTO> destinatarios)
        {
            // Mapeo y guardado
            return Ok(new { success = true, count = destinatarios.Count });
        }

        [HttpPost("paquetes")]
        public IActionResult SyncPaquetes([FromBody] List<PaqueteSyncDTO> paquetes)
        {
            // Mapeo y guardado
            return Ok(new { success = true, count = paquetes.Count });
        }
    }
}