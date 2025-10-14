using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManyBoxApi.Data;
using ManyBoxApi.Models;
using ManyBoxApi.DTOs;
using System.Security.Claims;
using System.Linq;
using ManyBoxApi.ViewModels;


namespace ManyBoxApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConversacionesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ConversacionesController(AppDbContext db)
        {
            _db = db;
        }

        private int GetUserId()
        {
            var idStr = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(idStr)) throw new UnauthorizedAccessException("No se encontró el id de usuario en el token.");
            return int.Parse(idStr);
        }

        // GET: api/conversaciones
        [HttpGet]
        public async Task<ActionResult<object>> GetMisConversaciones()
        {
            var userId = GetUserId();

            var conversaciones = await _db.Set<Conversacion>()
                .Include(c => c.Participantes)
                .Where(c => c.Participantes.Any(p => p.UsuarioId == userId))
                .OrderByDescending(c => c.FechaCreacion)
                .ToListAsync();

            // Traer los datos de los participantes para cada conversación
            var usuarioIds = conversaciones.SelectMany(c => c.Participantes.Select(p => p.UsuarioId)).Distinct().ToList();
            var usuarios = await _db.Set<Usuario>()
                .Where(u => usuarioIds.Contains(u.Id))
                .ToListAsync();

            // Traer empleados y sucursales para los usuarios
            var empleadoIds = usuarios.Where(u => u.EmpleadoId.HasValue).Select(u => u.EmpleadoId.Value).Distinct().ToList();
            var empleados = await _db.Set<Empleado>()
                .Where(e => empleadoIds.Contains(e.Id))
                .ToListAsync();
            var sucursalIds = empleados.Where(e => e.SucursalId.HasValue).Select(e => e.SucursalId.Value).Distinct().ToList();
            var sucursales = await _db.Set<Sucursal>()
                .Where(s => sucursalIds.Contains(s.Id))
                .ToListAsync();

            var usuariosVM = usuarios.Select(u => {
                var empleado = empleados.FirstOrDefault(e => e.Id == u.EmpleadoId);
                var sucursalNombre = empleado != null && empleado.SucursalId.HasValue
                    ? sucursales.FirstOrDefault(s => s.Id == empleado.SucursalId.Value)?.Nombre
                    : null;
                return new UsuarioContactoVM {
                    Id = u.Id,
                    Username = u.Username,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    SucursalNombre = sucursalNombre ?? string.Empty
                };
            }).ToList();

            var result = conversaciones.Select(c => new {
                c.Id,
                c.Tipo,
                c.Nombre,
                c.FechaCreacion,
                c.Archivada, // <-- Asegura que se incluya el campo Archivada
                Participantes = c.Participantes.Select(p => usuariosVM.First(u => u.Id == p.UsuarioId)).ToList()
            }).ToList();

            return Ok(result);
        }

        // POST: api/conversaciones
        [HttpPost]
        public async Task<ActionResult<object>> CrearConversacion([FromBody] CrearConversacionDTO req)
        {
            var creadorId = GetUserId();

            // Validar que existan los usuarios participantes
            var usuariosExistentes = await _db.Set<Usuario>()
                .Where(u => req.ParticipantesIds.Contains(u.Id) || u.Id == creadorId)
                .Select(u => u.Id)
                .ToListAsync();

            var participantesIdsValidos = req.ParticipantesIds.Distinct().Append(creadorId).Distinct()
                .Where(id => usuariosExistentes.Contains(id))
                .ToList();

            if (participantesIdsValidos.Count < 2) // Mínimo 2 participantes (creador + al menos 1 más)
            {
                return BadRequest("Debe haber al menos 2 participantes válidos en la conversación");
            }

            var conv = new Conversacion
            {
                Tipo = req.Tipo,
                Nombre = req.Nombre,
                CreadoPor = creadorId,
                FechaCreacion = DateTime.UtcNow
            };

            _db.Add(conv);
            await _db.SaveChangesAsync();

            // Crear participantes con fecha de unión
            var participantes = participantesIdsValidos
                .Select(uid => new ConversacionParticipante
                {
                    ConversacionId = conv.Id,
                    UsuarioId = uid,
                    Rol = uid == creadorId ? "admin" : "miembro", // El creador es admin
                    FechaUnion = DateTime.UtcNow
                });

            _db.AddRange(participantes);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMensajes), new { id = conv.Id }, new { conv.Id, conv.Tipo, conv.Nombre });
        }

        // GET: api/conversaciones/{id}/mensajes
        [HttpGet("{id}/mensajes")]
        public async Task<ActionResult<IEnumerable<MensajeVM>>> GetMensajes(int id, int pagina = 1, int pageSize = 50)
        {
            var userId = GetUserId();

            // Validar que el usuario pertenezca a la conversación
            var pertenece = await _db.Set<ConversacionParticipante>()
                .AnyAsync(p => p.ConversacionId == id && p.UsuarioId == userId);
            if (!pertenece) return Forbid();

            var mensajes = await _db.Set<MensajeChat>()
                .Where(m => m.ConversacionId == id && !m.Eliminado)
                .OrderByDescending(m => m.Id)
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MensajeVM
                {
                    Id = m.Id,
                    ConversacionId = m.ConversacionId,
                    UsuarioId = m.UsuarioId,
                    Contenido = m.Contenido,
                    Tipo = m.Tipo,
                    FechaCreacion = m.FechaCreacion,
                    Editado = m.Editado,
                    Eliminado = m.Eliminado,
                    LeidoPorMi = _db.Set<MensajeLeido>()
                        .Any(ml => ml.MensajeId == m.Id && ml.UsuarioId == userId && ml.Estado == "leido"),
                    ReplyToId = m.ReplyToId,
                    ArchivoUrl = m.ArchivoUrl,
                    ArchivoNombreOriginal = m.ArchivoNombreOriginal
                })
                .ToListAsync();

            return Ok(mensajes.OrderBy(m => m.Id));
        }

        // POST: api/conversaciones/{id}/mensajes
        [HttpPost("{id}/mensajes")]
        public async Task<ActionResult<MensajeVM>> EnviarMensaje(int id, [FromBody] EnviarMensajeRequest req)
        {
            var userId = GetUserId();

            var pertenece = await _db.Set<ConversacionParticipante>()
                .AnyAsync(p => p.ConversacionId == id && p.UsuarioId == userId);
            if (!pertenece) return Forbid();

            var mensaje = new MensajeChat
            {
                ConversacionId = id,
                UsuarioId = userId,
                Contenido = req.Contenido,
                Tipo = req.Tipo,
                FechaCreacion = DateTime.UtcNow,
                Editado = false,
                Eliminado = false,
                ReplyToId = req.ReplyToId,
                ArchivoUrl = req.ArchivoUrl,
                ArchivoNombreOriginal = req.ArchivoNombreOriginal
            };

            _db.Add(mensaje);
            await _db.SaveChangesAsync();

            // Estado "enviado" del emisor
            _db.Add(new MensajeLeido
            {
                MensajeId = mensaje.Id,
                UsuarioId = userId,
                Estado = "enviado",
                FechaEstado = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            var vm = new MensajeVM
            {
                Id = mensaje.Id,
                ConversacionId = mensaje.ConversacionId,
                UsuarioId = mensaje.UsuarioId,
                Contenido = mensaje.Contenido,
                Tipo = mensaje.Tipo,
                FechaCreacion = mensaje.FechaCreacion,
                Editado = mensaje.Editado,
                Eliminado = mensaje.Eliminado,
                LeidoPorMi = true,
                ReplyToId = mensaje.ReplyToId,
                ArchivoUrl = mensaje.ArchivoUrl,
                ArchivoNombreOriginal = mensaje.ArchivoNombreOriginal
            };

            return CreatedAtAction(nameof(GetMensajes), new { id }, vm);
        }

        // PUT: api/mensajes/{mensajeId}
        [HttpPut("~/api/mensajes/{mensajeId}")]
        public async Task<IActionResult> EditarMensaje(long mensajeId, [FromBody] EnviarMensajeRequest req)
        {
            var userId = GetUserId();

            var msg = await _db.Set<MensajeChat>().FirstOrDefaultAsync(m => m.Id == mensajeId);
            if (msg == null || msg.Eliminado) return NotFound();
            if (msg.UsuarioId != userId) return Forbid();

            msg.Contenido = req.Contenido;
            msg.Tipo = req.Tipo;
            msg.Editado = true;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/conversaciones/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> EliminarConversacion(int id)
        {
            var conv = await _db.Set<Conversacion>()
                .Include(c => c.Mensajes)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (conv == null) return NotFound();

            // Eliminado lógico: marcar todos los mensajes y la conversación como eliminados
            foreach (var msg in conv.Mensajes)
                msg.Eliminado = true;
            _db.Remove(conv);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE lógico: api/mensajes/{mensajeId}
        [HttpDelete("~/api/mensajes/{mensajeId}")]
        //[Authorize(Roles = "SuperAdmin,Admin")] // <-- Cambiado: permitir a cualquier usuario autenticado
        public async Task<IActionResult> BorrarMensaje(long mensajeId)
        {
            var userId = GetUserId();
            var msg = await _db.Set<MensajeChat>().FirstOrDefaultAsync(m => m.Id == mensajeId);
            if (msg == null || msg.Eliminado) return NotFound();
            // Permitir borrar si es el emisor o participante de la conversación
            var esParticipante = await _db.Set<ConversacionParticipante>()
                .AnyAsync(p => p.ConversacionId == msg.ConversacionId && p.UsuarioId == userId);
            if (msg.UsuarioId != userId && !esParticipante)
                return Forbid();
            msg.Eliminado = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/mensajes/{mensajeId}/leido
        [HttpPost("~/api/mensajes/{mensajeId}/leido")]
        public async Task<IActionResult> MarcarLeido(long mensajeId)
        {
            var userId = GetUserId();

            var existe = await _db.Set<MensajeLeido>()
                .FirstOrDefaultAsync(x => x.MensajeId == mensajeId && x.UsuarioId == userId);

            if (existe == null)
            {
                _db.Add(new MensajeLeido
                {
                    MensajeId = mensajeId,
                    UsuarioId = userId,
                    Estado = "leido",
                    FechaEstado = DateTime.UtcNow
                });
            }
            else
            {
                existe.Estado = "leido";
                existe.FechaEstado = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/conversaciones/buscar-directo?usuario1=1&usuario2=2
        [HttpGet("buscar-directo")]
        public async Task<IActionResult> BuscarConversacionDirecta(int usuario1, int usuario2)
        {
            var conv = await _db.Set<Conversacion>()
                .Where(c => c.Tipo == "directo")
                .Where(c => c.Participantes.Count == 2 &&
                            c.Participantes.Any(p => p.UsuarioId == usuario1) &&
                            c.Participantes.Any(p => p.UsuarioId == usuario2))
                .Include(c => c.Participantes)
                .FirstOrDefaultAsync();
            if (conv == null) return NotFound();

            // Mapear a ConversacionModel (incluyendo participantes)
            var participantes = await _db.Set<Usuario>()
                .Where(u => conv.Participantes.Select(p => p.UsuarioId).Contains(u.Id))
                .Select(u => new {
                    u.Id,
                    u.Username,
                    u.Nombre
                }).ToListAsync();

            return Ok(new {
                conv.Id,
                conv.Tipo,
                conv.Nombre,
                conv.FechaCreacion,
                Participantes = participantes
            });
        }

        // POST: api/conversaciones/{id}/marcar-leidos
        [HttpPost("{id}/marcar-leidos")]
        public async Task<IActionResult> MarcarMensajesComoLeidos(int id, [FromBody] MarcarLeidosRequest body)
        {
            int usuarioId = body.UsuarioId;
            var mensajes = await _db.Set<MensajeChat>()
                .Where(m => m.ConversacionId == id && !m.Eliminado)
                .Select(m => m.Id)
                .ToListAsync();

            var leidos = await _db.Set<MensajeLeido>()
                .Where(ml => mensajes.Contains(ml.MensajeId) && ml.UsuarioId == usuarioId)
                .ToListAsync();

            var ahora = DateTime.UtcNow;
            foreach (var mensajeId in mensajes)
            {
                var existente = leidos.FirstOrDefault(l => l.MensajeId == mensajeId);
                if (existente == null)
                {
                    _db.Add(new MensajeLeido
                    {
                        MensajeId = mensajeId,
                        UsuarioId = usuarioId,
                        Estado = "leido",
                        FechaEstado = ahora
                    });
                }
                else
                {
                    existente.Estado = "leido";
                    existente.FechaEstado = ahora;
                }
            }
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/conversaciones/{id}/archivar
        [HttpPatch("{id}/archivar")]
        public async Task<IActionResult> ArchivarConversacion(int id)
        {
            var conv = await _db.Set<Conversacion>().FirstOrDefaultAsync(c => c.Id == id);
            if (conv == null) return NotFound();
            conv.Archivada = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/conversaciones/{id}/desarchivar
        [HttpPatch("{id}/desarchivar")]
        public async Task<IActionResult> DesarchivarConversacion(int id)
        {
            var conv = await _db.Set<Conversacion>().FirstOrDefaultAsync(c => c.Id == id);
            if (conv == null) return NotFound();
            conv.Archivada = false;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/conversaciones/{id}/agregar-participantes
        [HttpPost("{id}/agregar-participantes")]
        public async Task<IActionResult> AgregarParticipantes(int id, [FromBody] List<int> nuevosParticipantesIds)
        {
            var userId = GetUserId();
            var conv = await _db.Set<Conversacion>()
                .Include(c => c.Participantes)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (conv == null) return NotFound();
            if (conv.Tipo != "grupo") return BadRequest("Solo se pueden agregar participantes a grupos");
            // Solo el creador o admin puede agregar
            var esAdmin = conv.Participantes.Any(p => p.UsuarioId == userId && p.Rol == "admin");
            if (!esAdmin) return Forbid();
            // Filtrar ids que ya están
            var yaEstan = conv.Participantes.Select(p => p.UsuarioId).ToHashSet();
            var idsAAgregar = nuevosParticipantesIds.Where(idp => !yaEstan.Contains(idp)).Distinct().ToList();
            if (!idsAAgregar.Any()) return BadRequest("No hay participantes nuevos para agregar");
            var usuarios = await _db.Set<Usuario>().Where(u => idsAAgregar.Contains(u.Id)).ToListAsync();
            var ahora = DateTime.UtcNow;
            var nuevos = usuarios.Select(u => new ConversacionParticipante
            {
                ConversacionId = conv.Id,
                UsuarioId = u.Id,
                Rol = "miembro",
                FechaUnion = ahora
            });
            _db.AddRange(nuevos);
            await _db.SaveChangesAsync();
            return Ok();
        }

        // POST: api/conversaciones/{id}/eliminar-participantes
        [HttpPost("{id}/eliminar-participantes")]
        public async Task<IActionResult> EliminarParticipantes(int id, [FromBody] List<int> participantesIds)
        {
            var userId = GetUserId();
            var conv = await _db.Set<Conversacion>()
                .Include(c => c.Participantes)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (conv == null) return NotFound();
            if (conv.Tipo != "grupo") return BadRequest("Solo se pueden eliminar participantes de grupos");
            // Solo el admin puede eliminar
            var esAdmin = conv.Participantes.Any(p => p.UsuarioId == userId && p.Rol == "admin");
            if (!esAdmin) return Forbid();
            // No permitir eliminarse a sí mismo
            var idsAEliminar = participantesIds.Where(pid => pid != userId).Distinct().ToList();
            if (idsAEliminar.Count != participantesIds.Count)
            {
                return BadRequest("No puedes eliminarte a ti mismo del grupo.");
            }
            if (!idsAEliminar.Any()) return BadRequest("No hay participantes válidos para eliminar");
            var participantes = conv.Participantes.Where(p => idsAEliminar.Contains(p.UsuarioId)).ToList();
            if (!participantes.Any()) return BadRequest("No se encontraron participantes para eliminar");
            _db.RemoveRange(participantes);
            await _db.SaveChangesAsync();
            return Ok();
        }

        // GET: api/conversaciones/{id}/es-admin
        [HttpGet("{id}/es-admin")]
        public async Task<IActionResult> EsAdmin(int id)
        {
            var userId = GetUserId();
            var esAdmin = await _db.Set<ConversacionParticipante>()
                .AnyAsync(p => p.ConversacionId == id && p.UsuarioId == userId && p.Rol == "admin");
            return Ok(new { esAdmin });
        }

        // PATCH: api/conversaciones/{id}/asignar-admins
        [HttpPatch("{id}/asignar-admins")]
        public async Task<IActionResult> AsignarAdmins(int id, [FromBody] AsignarAdminsRequest req)
        {
            var userId = GetUserId();
            var conv = await _db.Set<Conversacion>()
                .Include(c => c.Participantes)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (conv == null) return NotFound();
            if (conv.Tipo != "grupo") return BadRequest("Solo se pueden asignar administradores en grupos");
            // Solo el creador puede asignar admins
            if (conv.CreadoPor != userId) return Forbid();
            // El creador siempre debe ser admin y no puede ser removido
            var participantes = conv.Participantes.ToList();
            foreach (var p in participantes)
            {
                if (p.UsuarioId == conv.CreadoPor)
                {
                    p.Rol = "admin";
                    continue;
                }
                if (req.UsuarioIds.Contains(p.UsuarioId))
                    p.Rol = "admin";
                else
                    p.Rol = "miembro";
            }
            await _db.SaveChangesAsync();
            return Ok();
        }

        // GET: api/conversaciones/{id}/admins
        [HttpGet("{id}/admins")]
        public async Task<IActionResult> GetAdmins(int id)
        {
            var conv = await _db.Set<Conversacion>()
                .Include(c => c.Participantes)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (conv == null) return NotFound();
            var adminIds = conv.Participantes.Where(p => p.Rol == "admin").Select(p => p.UsuarioId).ToList();
            return Ok(adminIds);
        }

        // GET: api/conversaciones/{id}/creador
        [HttpGet("{id}/creador")]
        public async Task<IActionResult> GetCreadorId(int id)
        {
            var conv = await _db.Set<Conversacion>().FirstOrDefaultAsync(c => c.Id == id);
            if (conv == null) return NotFound();
            return Ok(new { creadorId = conv.CreadoPor });
        }

        // GET: api/mensajes/{mensajeId}/estados
        [HttpGet("~/api/mensajes/{mensajeId}/estados")]
        public async Task<IActionResult> GetEstadosMensaje(long mensajeId)
        {
            var estados = await _db.Set<MensajeLeido>()
                .Where(x => x.MensajeId == mensajeId)
                .Select(x => new { x.UsuarioId, x.Estado, x.FechaEstado })
                .ToListAsync();
            return Ok(estados);
        }

        // PATCH: api/conversaciones/{id}/cambiar-nombre
        [HttpPatch("{id}/cambiar-nombre")]
        public async Task<IActionResult> CambiarNombreGrupo(int id, [FromBody] CambiarNombreGrupoRequest req)
        {
            var userId = GetUserId();
            var conv = await _db.Set<Conversacion>().FirstOrDefaultAsync(c => c.Id == id);
            if (conv == null) return NotFound();
            if (conv.Tipo != "grupo") return BadRequest("Solo se puede cambiar el nombre de grupos");
            if (conv.CreadoPor != userId) return Forbid();
            if (string.IsNullOrWhiteSpace(req.Nombre)) return BadRequest("El nombre no puede estar vacío");
            conv.Nombre = req.Nombre;
            await _db.SaveChangesAsync();
            return Ok();
        }

        // GET: api/usuarios/{usuarioId}/mensajes/no-leidos/count
        [HttpGet("~/api/usuarios/{usuarioId}/mensajes/no-leidos/count")]
        public async Task<ActionResult<int>> GetMensajesNoLeidosCount(int usuarioId)
        {
            // Cuenta mensajes en conversaciones donde el usuario participa y no ha marcado como leido
            var conversacionesIds = await _db.Set<ConversacionParticipante>()
                .Where(p => p.UsuarioId == usuarioId)
                .Select(p => p.ConversacionId)
                .ToListAsync();
            var mensajesNoLeidos = await _db.Set<MensajeChat>()
                .Where(m => conversacionesIds.Contains(m.ConversacionId) && !m.Eliminado)
                .Where(m => !_db.Set<MensajeLeido>().Any(ml => ml.MensajeId == m.Id && ml.UsuarioId == usuarioId && ml.Estado == "leido") && m.UsuarioId != usuarioId)
                .CountAsync();
            return Ok(mensajesNoLeidos);
        }

        // PUT: api/usuarios/{usuarioId}/mensajes/marcar-todos-leidos
        [HttpPut("~/api/usuarios/{usuarioId}/mensajes/marcar-todos-leidos")]
        public async Task<IActionResult> MarcarTodosMensajesLeidos(int usuarioId)
        {
            var conversacionesIds = await _db.Set<ConversacionParticipante>()
                .Where(p => p.UsuarioId == usuarioId)
                .Select(p => p.ConversacionId)
                .ToListAsync();
            var mensajes = await _db.Set<MensajeChat>()
                .Where(m => conversacionesIds.Contains(m.ConversacionId) && !m.Eliminado && m.UsuarioId != usuarioId)
                .Select(m => m.Id)
                .ToListAsync();
            var leidos = await _db.Set<MensajeLeido>()
                .Where(ml => mensajes.Contains(ml.MensajeId) && ml.UsuarioId == usuarioId)
                .ToListAsync();
            var ahora = DateTime.UtcNow;
            foreach (var mensajeId in mensajes)
            {
                var existente = leidos.FirstOrDefault(l => l.MensajeId == mensajeId);
                if (existente == null)
                {
                    _db.Add(new MensajeLeido
                    {
                        MensajeId = mensajeId,
                        UsuarioId = usuarioId,
                        Estado = "leido",
                        FechaEstado = ahora
                    });
                }
                else
                {
                    existente.Estado = "leido";
                    existente.FechaEstado = ahora;
                }
            }
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // NUEVO: Obtener conteo de mensajes enviados por usuario en una conversación
        // GET: api/conversaciones/{convId}/mensajes-por-usuario
        [HttpGet("{convId}/mensajes-por-usuario")]
        public async Task<ActionResult<IEnumerable<object>>> GetMensajesPorUsuario(int convId)
        {
            var userId = GetUserId();
            var mensajes = await _db.Set<MensajeChat>()
                .Where(m => m.ConversacionId == convId && m.UsuarioId != userId && !m.Eliminado)
                .GroupBy(m => m.UsuarioId)
                .Select(g => new {
                    UsuarioId = g.Key,
                    Cantidad = g.Count(m =>
                        !_db.Set<MensajeLeido>().Any(ml => ml.MensajeId == m.Id && ml.UsuarioId == userId && ml.Estado == "leido")
                    )
                })
                .ToListAsync();
            return Ok(mensajes);
        }
    }
}