using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManyBoxApi.Data;
using ManyBoxApi.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class NotificacionesController : ControllerBase
{
    private readonly AppDbContext _context;
    public NotificacionesController(AppDbContext context)
    {
        _context = context;
    }

    // 1. Listar notificaciones con filtros generales
    // GET: api/notificaciones?usuarioId=1&estado=activa&prioridad=Alta&tipo=mensaje
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notificacion>>> GetNotificaciones([FromQuery] int? usuarioId, [FromQuery] string estado = null, [FromQuery] string prioridad = null, [FromQuery] string tipo = null)
    {
        var query = _context.Notificaciones
            .Include(n => n.Destinatarios)
            .Include(n => n.Entregas)
            .Where(n => n.DeletedAt == null);

        if (usuarioId.HasValue)
        {
            query = query.Where(n => n.Destinatarios.Any(d => d.UsuarioId == usuarioId));
        }
        if (!string.IsNullOrEmpty(estado))
        {
            query = query.Where(n => n.Estado == estado);
        }
        if (!string.IsNullOrEmpty(prioridad))
        {
            query = query.Where(n => n.Prioridad == prioridad);
        }
        if (!string.IsNullOrEmpty(tipo))
        {
            query = query.Where(n => n.Tipo == tipo);
        }
        return await query.OrderByDescending(n => n.FechaCreacion).ToListAsync();
    }

    // 2. Detalle de notificación
    // GET: api/notificaciones/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Notificacion>> GetNotificacion(int id)
    {
        var notificacion = await _context.Notificaciones
            .Include(n => n.Destinatarios)
            .Include(n => n.Entregas)
            .FirstOrDefaultAsync(n => n.Id == id && n.DeletedAt == null);
        if (notificacion == null)
            return NotFound();
        return notificacion;
    }

    // 3. Editar notificación
    // PUT: api/notificaciones/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> EditarNotificacion(int id, [FromBody] Notificacion model)
    {
        var notificacion = await _context.Notificaciones.FindAsync(id);
        if (notificacion == null || notificacion.DeletedAt != null)
            return NotFound();
        notificacion.Titulo = model.Titulo;
        notificacion.Mensaje = model.Mensaje;
        notificacion.Prioridad = model.Prioridad;
        notificacion.Tipo = model.Tipo;
        notificacion.Datos = model.Datos;
        notificacion.Expiracion = model.Expiracion;
        notificacion.Estado = model.Estado;
        notificacion.Idioma = model.Idioma;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // 4. Soft delete notificación
    // DELETE: api/notificaciones/{id}/soft
    [HttpDelete("{id}/soft")]
    public async Task<IActionResult> SoftDeleteNotificacion(int id)
    {
        var notificacion = await _context.Notificaciones.FindAsync(id);
        if (notificacion == null)
            return NotFound();
        notificacion.DeletedAt = DateTime.UtcNow;
        notificacion.Estado = "eliminada";
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // 5. Listar destinatarios de una notificación
    // GET: api/notificaciones/{id}/destinatarios
    [HttpGet("{id}/destinatarios")]
    public async Task<ActionResult<IEnumerable<NotificacionDestinatario>>> GetDestinatarios(int id)
    {
        var destinatarios = await _context.NotificacionDestinatarios.Where(d => d.NotificacionId == id).ToListAsync();
        return destinatarios;
    }

    // 6. Agregar destinatario(s) a una notificación
    // POST: api/notificaciones/{id}/destinatarios
    [HttpPost("{id}/destinatarios")]
    public async Task<IActionResult> AddDestinatarios(int id, [FromBody] List<NotificacionDestinatario> nuevos)
    {
        foreach (var d in nuevos)
        {
            d.NotificacionId = id;
            _context.NotificacionDestinatarios.Add(d);
        }
        await _context.SaveChangesAsync();
        return Ok();
    }

    // 7. Eliminar destinatario específico
    // DELETE: api/notificaciones/{id}/destinatarios/{destinatarioId}
    [HttpDelete("{id}/destinatarios/{destinatarioId}")]
    public async Task<IActionResult> DeleteDestinatario(int id, int destinatarioId)
    {
        var dest = await _context.NotificacionDestinatarios.FirstOrDefaultAsync(d => d.NotificacionId == id && d.Id == destinatarioId);
        if (dest == null)
            return NotFound();
        _context.NotificacionDestinatarios.Remove(dest);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // 8. Listar entregas de una notificación
    // GET: api/notificaciones/{id}/entregas
    [HttpGet("{id}/entregas")]
    public async Task<ActionResult<IEnumerable<NotificacionEntrega>>> GetEntregas(int id)
    {
        var entregas = await _context.NotificacionEntregas.Where(e => e.NotificacionId == id).ToListAsync();
        return entregas;
    }

    // 9. Detalle de una entrega
    // GET: api/entregas/{entregaId}
    [HttpGet("~/api/entregas/{entregaId}")]
    public async Task<ActionResult<NotificacionEntrega>> GetEntrega(int entregaId)
    {
        var entrega = await _context.NotificacionEntregas.FindAsync(entregaId);
        if (entrega == null)
            return NotFound();
        return entrega;
    }

    // 10. Actualizar estado de entrega
    // PUT: api/entregas/{entregaId}
    [HttpPut("~/api/entregas/{entregaId}")]
    public async Task<IActionResult> UpdateEntrega(int entregaId, [FromBody] NotificacionEntrega model)
    {
        var entrega = await _context.NotificacionEntregas.FindAsync(entregaId);
        if (entrega == null)
            return NotFound();
        entrega.Estado = model.Estado;
        entrega.FechaLeido = model.FechaLeido;
        entrega.Canal = model.Canal;
        entrega.Error = model.Error;
        entrega.UltimoError = model.UltimoError;
        entrega.DeviceId = model.DeviceId;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // 11. Notificaciones para usuario (filtros)
    // GET: api/usuarios/{usuarioId}/notificaciones
    [HttpGet("~/api/usuarios/{usuarioId}/notificaciones")]
    public async Task<ActionResult<IEnumerable<Notificacion>>> GetNotificacionesUsuarioV2(int usuarioId, [FromQuery] string estado = null, [FromQuery] string prioridad = null)
    {
        var notificaciones = await _context.Notificaciones
            .Include(n => n.Destinatarios)
            .Where(n => n.Destinatarios.Any(d => d.UsuarioId == usuarioId) && n.DeletedAt == null)
            .ToListAsync();
        if (!string.IsNullOrEmpty(estado))
            notificaciones = notificaciones.Where(n => n.Estado == estado).ToList();
        if (!string.IsNullOrEmpty(prioridad))
            notificaciones = notificaciones.Where(n => n.Prioridad == prioridad).ToList();
        return notificaciones;
    }

    // 12. Marcar una notificación como leída para usuario
    // PUT: api/usuarios/{usuarioId}/notificaciones/{notificacionId}/leida
    [HttpPut("~/api/usuarios/{usuarioId}/notificaciones/{notificacionId}/leida")]
    public async Task<IActionResult> MarcarLeidaUsuario(int usuarioId, int notificacionId)
    {
        var entrega = await _context.NotificacionEntregas.FirstOrDefaultAsync(e => e.NotificacionId == notificacionId && e.UsuarioId == usuarioId);
        if (entrega == null)
            return NotFound();
        entrega.Estado = "leida";
        entrega.FechaLeido = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // 13. Marcar todas como leídas para usuario
    // PUT: api/usuarios/{usuarioId}/notificaciones/leidas
    [HttpPut("~/api/usuarios/{usuarioId}/notificaciones/leidas")]
    public async Task<IActionResult> MarcarTodasLeidasUsuario(int usuarioId)
    {
        var entregas = await _context.NotificacionEntregas.Where(e => e.UsuarioId == usuarioId && e.Estado != "leida").ToListAsync();
        foreach (var entrega in entregas)
        {
            entrega.Estado = "leida";
            entrega.FechaLeido = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // 14. Eliminar notificación para usuario (soft delete de entrega)
    // DELETE: api/usuarios/{usuarioId}/notificaciones/{notificacionId}
    [HttpDelete("~/api/usuarios/{usuarioId}/notificaciones/{notificacionId}")]
    public async Task<IActionResult> DeleteNotificacionUsuario(int usuarioId, int notificacionId)
    {
        var entrega = await _context.NotificacionEntregas.FirstOrDefaultAsync(e => e.NotificacionId == notificacionId && e.UsuarioId == usuarioId);
        if (entrega == null)
            return NotFound();
        entrega.Estado = "eliminada";
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // 15. Estadísticas de notificaciones
    // GET: api/notificaciones/estadisticas
    [HttpGet("estadisticas")]
    public async Task<ActionResult<object>> GetEstadisticas()
    {
        var total = await _context.Notificaciones.CountAsync(n => n.DeletedAt == null);
        var leidas = await _context.NotificacionEntregas.CountAsync(e => e.Estado == "leida");
        var pendientes = await _context.NotificacionEntregas.CountAsync(e => e.Estado == "pendiente");
        var porPrioridad = await _context.Notificaciones.GroupBy(n => n.Prioridad).Select(g => new { Prioridad = g.Key, Cantidad = g.Count() }).ToListAsync();
        return new { total, leidas, pendientes, porPrioridad };
    }

    // 16. Enviar notificación de prueba
    // POST: api/notificaciones/test
    [HttpPost("test")]
    public async Task<IActionResult> TestNotificacion([FromBody] CrearNotificacionRequest req)
    {
        var notificacion = new Notificacion
        {
            Tipo = req.Tipo,
            Titulo = req.Titulo ?? "Test",
            Mensaje = req.Mensaje ?? "Mensaje de prueba",
            Prioridad = req.Prioridad ?? "Media",
            FechaCreacion = DateTime.UtcNow,
            Estado = "activa"
        };
        _context.Notificaciones.Add(notificacion);
        await _context.SaveChangesAsync();
        return Ok(notificacion);
    }

    // 17. Listar plantillas de notificación (dummy)
    // GET: api/notificaciones/plantillas
    [HttpGet("plantillas")]
    public ActionResult<IEnumerable<object>> GetPlantillas()
    {
        return new[]
        {
            new { Id = 1, Nombre = "Bienvenida", Contenido = "¡Bienvenido, {nombre}!" },
            new { Id = 2, Nombre = "Alerta", Contenido = "Tienes una alerta importante." }
        };
    }

    // GET: api/notificaciones/usuario/{usuarioId}
    [HttpGet("usuario/{usuarioId}")]
    public async Task<ActionResult<IEnumerable<NotificacionUsuarioVM>>> GetNotificacionesUsuario(int usuarioId)
    {
        // 1. Buscamos las notificaciones para el usuario
        var destinatarios = await _context.NotificacionDestinatarios
            .Include(nd => nd.Notificacion)
            .Where(nd => nd.UsuarioId == usuarioId)
            .ToListAsync();

        var resultado = new List<NotificacionUsuarioVM>();

        foreach (var nd in destinatarios)
        {
            // 2. Buscamos la entrega para esa notificación y usuario
            var entrega = await _context.NotificacionEntregas
                .FirstOrDefaultAsync(e => e.NotificacionId == nd.NotificacionId && e.UsuarioId == usuarioId);

            resultado.Add(new NotificacionUsuarioVM
            {
                Id = nd.Notificacion.Id,
                Tipo = nd.Notificacion.Tipo,
                Titulo = nd.Notificacion.Titulo,
                Mensaje = nd.Notificacion.Mensaje,
                Prioridad = nd.Notificacion.Prioridad,
                FechaCreacion = nd.Notificacion.FechaCreacion,
                Datos = nd.Notificacion.Datos,
                Expiracion = nd.Notificacion.Expiracion,
                Leido = entrega != null && entrega.Estado == "leida"
            });
        }

        return Ok(resultado);
    }

    // ViewModel recomendado
    public class NotificacionUsuarioVM
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public string Prioridad { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string? Datos { get; set; }
        public DateTime? Expiracion { get; set; }
        public bool Leido { get; set; }
    }

    // PUT: api/notificaciones/entrega/{notificacionId}/leida?usuarioId=8
    [HttpPut("entrega/{notificacionId}/leida")]
    public async Task<IActionResult> MarcarComoLeida(int notificacionId, int usuarioId)
    {
        var entrega = await _context.NotificacionEntregas
            .FirstOrDefaultAsync(e => e.NotificacionId == notificacionId && e.UsuarioId == usuarioId);

        // Si no existe la entrega, la creamos
        if (entrega == null)
        {
            entrega = new NotificacionEntrega
            {
                NotificacionId = notificacionId,
                UsuarioId = usuarioId,
                Canal = "ws",
                Estado = "leida",
                FechaLeido = DateTime.Now
            };
            _context.NotificacionEntregas.Add(entrega);
        }
        else
        {
            entrega.Estado = "leida";
            entrega.FechaLeido = DateTime.Now;
        }

        // Guardamos el cambio en la entrega
        await _context.SaveChangesAsync();

        // Ahora revisamos si TODAS las entregas de esta notificación ya están leídas
        bool todasLeidas = await _context.NotificacionEntregas
            .Where(e => e.NotificacionId == notificacionId)
            .AllAsync(e => e.Estado == "leida");

        if (todasLeidas)
        {
            var notificacion = await _context.Notificaciones.FindAsync(notificacionId);
            if (notificacion != null)
            {
                // No hacemos nada aquí, ya que notificacion.Leido es una propiedad calculada
            }
        }

        return NoContent();
    }

    // POST: api/notificaciones
    [HttpPost]
    public async Task<ActionResult<Notificacion>> CrearNotificacion([FromBody] Notificacion notificacion)
    {
        notificacion.FechaCreacion = DateTime.UtcNow;
        _context.Notificaciones.Add(notificacion);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(CrearNotificacion), new { id = notificacion.Id }, notificacion);
    }

    // POST: api/notificaciones/entrega
    [HttpPost("entrega")]
    public async Task<ActionResult<NotificacionEntrega>> CrearEntrega([FromBody] NotificacionEntrega entrega)
    {
        entrega.FechaEnvio = DateTime.UtcNow;
        _context.NotificacionEntregas.Add(entrega);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(CrearEntrega), new { id = entrega.Id }, entrega);
    }
    // DELETE: api/notificaciones/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> BorrarNotificacion(int id)
    {
        var notificacion = await _context.Notificaciones
            .Include(n => n.Entregas)
            .Include(n => n.Destinatarios)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (notificacion == null)
            return NotFound();

        // No necesitas borrar manualmente las hijas si tienes Cascade Delete configurado
        _context.Notificaciones.Remove(notificacion);

        await _context.SaveChangesAsync();
        return NoContent();
    }
    // endpoin para crear notificaciones con destinatarios y entregas
    [HttpPost("crear-completa")]
    [Authorize(Roles = "SuperAdmin")] // Solo SuperAdmin puede crear notificaciones
    public async Task<ActionResult> CrearNotificacionCompleta([FromBody] CrearNotificacionRequest request)
    {
        // 1. Obtener la sucursal del usuario (usuario -> empleado -> sucursal)
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == request.UsuarioId);
        if (usuario == null || usuario.EmpleadoId == null)
        {
            return BadRequest(new { message = $"El usuario con ID {request.UsuarioId} no existe o no tiene empleado asociado." });
        }
        var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.Id == usuario.EmpleadoId);
        if (empleado == null || empleado.SucursalId == null)
        {
            return BadRequest(new { message = $"El empleado del usuario no tiene sucursal asignada." });
        }
        var sucursalId = empleado.SucursalId.Value;

        // Validar que el usuario autenticado sea SuperAdmin (rol_id == 1)
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return Forbid();
        var usuarioActual = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == userId);
        if (usuarioActual == null || usuarioActual.RolId != 1)
            return Forbid();

        // 2. Crear la notificación
        var notificacion = new Notificacion
        {
            Tipo = request.Tipo,
            Titulo = request.Titulo,
            Mensaje = request.Mensaje,
            Prioridad = request.Prioridad,
            FechaCreacion = DateTime.UtcNow,
            Datos = request.Datos,
            Expiracion = request.Expiracion
        };
        _context.Notificaciones.Add(notificacion);
        await _context.SaveChangesAsync();

        // 3. Obtener RolId válido para el destinatario
        int rolId = request.RolId;
        if (rolId <= 0)
        {
            var usuarioDest = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == request.UsuarioId);
            if (usuarioDest != null)
                rolId = usuarioDest.RolId;
        }

        // 4. Crear destinatario
        var destinatario = new NotificacionDestinatario
        {
            NotificacionId = notificacion.Id,
            UsuarioId = request.UsuarioId,
            SucursalId = sucursalId,
            RolId = rolId
        };
        _context.NotificacionDestinatarios.Add(destinatario);
        await _context.SaveChangesAsync();

        // 5. Crear entrega
        var entrega = new NotificacionEntrega
        {
            NotificacionId = notificacion.Id,
            UsuarioId = request.UsuarioId,
            Canal = "ws",
            Estado = "pendiente",
            FechaEnvio = DateTime.UtcNow
        };
        _context.NotificacionEntregas.Add(entrega);
        await _context.SaveChangesAsync();

        return Ok(new { notificacion.Id });
    }

    // 18. Obtener cantidad de notificaciones no leídas para usuario
    // GET: api/usuarios/{usuarioId}/notificaciones/no-leidas/count
    [HttpGet("~/api/usuarios/{usuarioId}/notificaciones/no-leidas/count")]
    public async Task<ActionResult<int>> GetNotificacionesNoLeidasCount(int usuarioId)
    {
        var count = await _context.NotificacionEntregas.CountAsync(e => e.UsuarioId == usuarioId && e.Estado != "leida");
        return Ok(count);
    }

    // 19. Marcar todas las notificaciones como leídas para usuario
    // PUT: api/usuarios/{usuarioId}/notificaciones/marcar-todas-leidas
    [HttpPut("~/api/usuarios/{usuarioId}/notificaciones/marcar-todas-leidas")]
    public async Task<IActionResult> MarcarTodasNotificacionesLeidas(int usuarioId)
    {
        var entregas = await _context.NotificacionEntregas.Where(e => e.UsuarioId == usuarioId && e.Estado != "leida").ToListAsync();
        foreach (var entrega in entregas)
        {
            entrega.Estado = "leida";
            entrega.FechaLeido = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
        return NoContent();
    }
}