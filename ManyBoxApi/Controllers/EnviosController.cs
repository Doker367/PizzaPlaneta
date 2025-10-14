using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ManyBoxApi.Services;
using ManyBoxApi.DTOs;
using ManyBoxApi.Models;
using Microsoft.EntityFrameworkCore;
using ManyBoxApi.Data;
using System;
using Microsoft.AspNetCore.SignalR;
using System.Linq;

namespace ManyBoxApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnviosController : ControllerBase
    {
        private readonly IEnvioService _envioService;
        private readonly AppDbContext _context;

        public EnviosController(IEnvioService envioService, AppDbContext context)
        {
            _envioService = envioService;
            _context = context;
        }

        // GET: api/envios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetEnvios()
        {
            var envios = await _context.Envios
                .Include(e => e.Empleado)
                .ToListAsync();
            var result = envios.Select(e => new {
                e.Id,
                GuiaRastreo = e.GuiaRastreo,
                FechaEntrega = e.FechaEntrega,
                EmpleadoId = e.EmpleadoId,
                EmpleadoNombre = e.Empleado != null ? e.Empleado.Nombre + " " + e.Empleado.Apellido : null
            }).ToList();
            return Ok(result);
        }

        // GET: api/envios/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<EnvioDto>> GetEnvio(int id)
        {
            var envio = await _envioService.GetEnvioByIdAsync(id);
            if (envio == null)
                return NotFound();
            return Ok(envio);
        }

        // NUEVO: estado actual del envío según seguimiento
        // GET: api/envios/{id}/estado-actual
        [HttpGet("{id}/estado-actual")]
        public async Task<ActionResult<object>> GetEstadoActual(int id)
        {
            var envioExiste = await _context.Envios.AnyAsync(e => e.Id == id);
            if (!envioExiste) return NotFound();

            var ultimo = await _context.SeguimientoPaquete
                .Where(s => s.EnvioId == id)
                .OrderByDescending(s => s.FechaStatus)
                .ThenByDescending(s => s.Id)
                .FirstOrDefaultAsync();

            var estado = ultimo?.Status ?? "Asignado";
            var entregado = string.Equals(estado, "Entregado", StringComparison.OrdinalIgnoreCase)
                            || string.Equals(estado, "Incidencia Cerrada", StringComparison.OrdinalIgnoreCase);

            return Ok(new { envioId = id, estado, fechaStatus = ultimo?.FechaStatus, entregado });
        }

        // NUEVO: verificar si puede editar fecha de entrega (no si está entregado)
        // GET: api/envios/{id}/puede-editar-fecha
        [HttpGet("{id}/puede-editar-fecha")]
        public async Task<ActionResult<object>> PuedeEditarFecha(int id)
        {
            var ultimo = await _context.SeguimientoPaquete
                .Where(s => s.EnvioId == id)
                .OrderByDescending(s => s.FechaStatus)
                .ThenByDescending(s => s.Id)
                .FirstOrDefaultAsync();
            var entregado = ultimo != null && (
                string.Equals(ultimo.Status, "Entregado", StringComparison.OrdinalIgnoreCase)
                || string.Equals(ultimo.Status, "Incidencia Cerrada", StringComparison.OrdinalIgnoreCase)
            );
            return Ok(new { envioId = id, puedeEditar = !entregado });
        }

        // POST: api/envios
        [HttpPost]
        public async Task<ActionResult<EnvioDto>> CrearEnvio([FromBody] CrearEnvioRequest request)
        {
            var nuevoEnvio = await _envioService.CreateEnvioAsync(request);
            return CreatedAtAction(nameof(GetEnvio), new { id = nuevoEnvio.Id }, nuevoEnvio);
        }

        // POST: api/envios/asignar
        [HttpPost("asignar")]
        public async Task<IActionResult> AsignarEnvio([FromBody] AsignarEnvioRequest request)
        {
            if (request == null || request.VentaId <= 0 || string.IsNullOrWhiteSpace(request.CodigoSeguimiento))
                return BadRequest("Datos de asignación inválidos.");

            var venta = await _context.Ventas.FirstOrDefaultAsync(v => v.Id == request.VentaId);
            if (venta == null)
                return NotFound($"Venta {request.VentaId} no encontrada");

            // Evitar duplicados: si ya hay envío para esta venta, devolver conflicto
            var yaExiste = await _context.Envios.AnyAsync(e => e.VentaId == request.VentaId);
            if (yaExiste)
                return Conflict(new { message = "La venta ya tiene un envío asignado." });

            // Validar empleado si viene
            if (request.EmpleadoId != 0)
            {
                var empExists = await _context.Empleados.AnyAsync(e => e.Id == request.EmpleadoId);
                if (!empExists)
                    return BadRequest("Empleado no válido");
            }

            var envio = new Envio
            {
                GuiaRastreo = request.CodigoSeguimiento,
                EmpleadoId = request.EmpleadoId == 0 ? null : request.EmpleadoId,
                FechaEntrega = request.FechaEntrega,
                VentaId = request.VentaId
            };

            _context.Envios.Add(envio);
            await _context.SaveChangesAsync();

            // Primer seguimiento: Asignado
            _context.SeguimientoPaquete.Add(new SeguimientoPaquete
            {
                EnvioId = envio.Id,
                Status = "Asignado",
                FechaStatus = DateTime.Now
            });
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEnvio), new { id = envio.Id }, new { envio.Id, envio.GuiaRastreo });
        }

        // PUT: api/envios/{id}/fecha-entrega
        [HttpPut("{id}/fecha-entrega")]
        public async Task<IActionResult> ActualizarFechaEntrega(int id, [FromBody] FechaEntregaRequest request)
        {
            var envio = await _context.Envios.FindAsync(id);
            if (envio == null)
                return NotFound();

            // Verifica último estado en seguimiento; si es 'Entregado' o 'Incidencia Cerrada' no se permite cambio
            var ultimo = await _context.SeguimientoPaquete
                .Where(s => s.EnvioId == id)
                .OrderByDescending(s => s.FechaStatus)
                .ThenByDescending(s => s.Id)
                .FirstOrDefaultAsync();
            if (ultimo != null && (
                    string.Equals(ultimo.Status, "Entregado", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(ultimo.Status, "Incidencia Cerrada", StringComparison.OrdinalIgnoreCase)))
            {
                return Conflict(new { message = "El envío ya fue entregado o tiene incidencia cerrada. No se puede modificar la fecha de entrega." });
            }

            envio.FechaEntrega = request.FechaEntrega;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/envios/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnvio(int id)
        {
            var envio = await _context.Envios.FindAsync(id);
            if (envio == null)
                return NotFound();
            _context.Envios.Remove(envio);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/envios/ventas-no-asignadas?usuarioId=1&rol=admin
        [HttpGet("ventas-no-asignadas")]
        public async Task<ActionResult<IEnumerable<object>>> GetVentasNoAsignadas([FromQuery] int usuarioId, [FromQuery] string rol)
        {
            rol = rol.ToLower();
            var ventasQuery = _context.Ventas
                .Include(v => v.Destinatario)
                .Include(v => v.Envios);

            if (rol == "superadmin")
            {
                var ventas = await ventasQuery.Where(v => !v.Envios.Any()).ToListAsync();
                return Ok(ventas.Select(v => new
                {
                    v.Id,
                    v.Folio,
                    Destinatario = v.Destinatario != null ? new { v.Destinatario.Nombre } : null
                }));
            }
            else if (rol == "admin")
            {
                var admin = await _context.Usuarios.Include(u => u.Empleado).FirstOrDefaultAsync(u => u.Id == usuarioId);
                if (admin == null || admin.Empleado == null || admin.Empleado.SucursalId == null)
                    return Forbid();
                int sucursalId = admin.Empleado.SucursalId.Value;
                var ventas = await ventasQuery.Where(v => !v.Envios.Any() && v.Empleado_Id != null && _context.Empleados.Any(e => e.Id == v.Empleado_Id && e.SucursalId == sucursalId)).ToListAsync();
                return Ok(ventas.Select(v => new
                {
                    v.Id,
                    v.Folio,
                    Destinatario = v.Destinatario != null ? new { v.Destinatario.Nombre } : null
                }));
            }
            return Forbid();
        }

        // --- NUEVOS ENDPOINTS DE PAQUETES ASIGNADOS ---
        // GET: api/envios/asignados/empleado/{empleadoId}
        [HttpGet("asignados/empleado/{empleadoId}")]
        public async Task<ActionResult<IEnumerable<GuiaChoferDto>>> GetPaquetesAsignadosAEmpleado(int empleadoId)
        {
            var envios = await _context.Envios
                .Include(e => e.Venta)
                .Where(e => e.EmpleadoId == empleadoId)
                .ToListAsync();

            var result = new List<GuiaChoferDto>();
            foreach (var envio in envios)
            {
                string destinatario = envio.Venta?.Destinatario?.Nombre ?? $"Venta #{envio.VentaId}";
                var seguimientos = await _context.SeguimientoPaquete
                    .Where(s => s.EnvioId == envio.Id)
                    .OrderBy(s => s.Id)
                    .ToListAsync();
                var estados = new List<EstadoGuiaDto>
                {
                    new EstadoGuiaDto { Titulo = "En preparación", Descripcion = "Paquete en sucursal.", FechaHora = envio.Venta?.Fecha ?? DateTime.MinValue },
                    new EstadoGuiaDto { Titulo = "En camino", Descripcion = "El chofer tiene el paquete.", FechaHora = DateTime.MinValue },
                    new EstadoGuiaDto { Titulo = "Último tramo", Descripcion = "El chofer está por entregar el paquete.", FechaHora = DateTime.MinValue },
                    new EstadoGuiaDto { Titulo = "Entregado", Descripcion = "El paquete ha sido entregado.", FechaHora = DateTime.MinValue }
                };
                foreach (var seg in seguimientos)
                {
                    int idx = seg.Status switch
                    {
                        "Asignado" => 0,
                        "En camino" => 1,
                        "Último tramo" => 2,
                        "Entregado" => 3,
                        _ => -1
                    };
                    if (idx >= 0 && idx < estados.Count)
                        estados[idx].FechaHora = seg.FechaStatus;
                }
                int estadoActual = 0;
                var seguimientoUltimo = seguimientos.OrderByDescending(s => s.FechaStatus).FirstOrDefault();
                if (seguimientoUltimo != null)
                {
                    estadoActual = seguimientoUltimo.Status switch
                    {
                        "Asignado" => 0,
                        "En camino" => 1,
                        "Último tramo" => 2,
                        "Entregado" => 3,
                        _ => 0
                    };
                }
                result.Add(new GuiaChoferDto
                {
                    EnvioId = envio.Id,
                    GuiaRastreo = envio.GuiaRastreo,
                    Destinatario = destinatario,
                    VentaId = envio.VentaId,
                    Estados = estados,
                    EstadoActual = estadoActual
                });
            }
            return Ok(result);
        }

        // GET: api/envios/asignados
        [HttpGet("asignados")]
        public async Task<ActionResult<IEnumerable<object>>> GetPaquetesAsignados([FromQuery] int usuarioId, [FromQuery] string rol)
        {
            rol = rol.ToLower();
            if (rol == "superadmin")
            {
                var envios = await _context.Envios
                    .Include(e => e.Empleado)
                    .Include(e => e.Venta)
                    .Where(e => e.EmpleadoId != null)
                    .ToListAsync();
                var result = envios.Select(e => new {
                    e.Id,
                    GuiaRastreo = e.GuiaRastreo,
                    FechaEntrega = e.FechaEntrega,
                    EmpleadoId = e.EmpleadoId,
                    EmpleadoNombre = e.Empleado != null ? e.Empleado.Nombre + " " + e.Empleado.Apellido : null,
                    VentaId = e.VentaId
                }).ToList();
                return Ok(result);
            }
            else if (rol == "admin")
            {
                var admin = await _context.Usuarios.Include(u => u.Empleado).FirstOrDefaultAsync(u => u.Id == usuarioId);
                if (admin == null || admin.Empleado == null || admin.Empleado.SucursalId == null)
                    return Forbid();
                int sucursalId = admin.Empleado.SucursalId.Value;
                var envios = await _context.Envios
                    .Include(e => e.Empleado)
                    .Include(e => e.Venta)
                    .Where(e => e.EmpleadoId != null &&
                                e.Venta != null &&
                                e.Venta.Empleado_Id != null &&
                                _context.Empleados.Any(emp => emp.Id == e.Venta.Empleado_Id && emp.SucursalId == sucursalId))
                    .ToListAsync();
                var result = envios.Select(e => new {
                    e.Id,
                    GuiaRastreo = e.GuiaRastreo,
                    FechaEntrega = e.FechaEntrega,
                    EmpleadoId = e.EmpleadoId,
                    EmpleadoNombre = e.Empleado != null ? e.Empleado.Nombre + " " + e.Empleado.Apellido : null,
                    VentaId = e.VentaId
                }).ToList();
                return Ok(result);
            }
            return Forbid();
        }

        // GET: api/envios/no-asignados
        [HttpGet("no-asignados")]
        public async Task<ActionResult<IEnumerable<object>>> GetPaquetesNoAsignados()
        {
            var envios = await _context.Envios
                .Include(e => e.Venta)
                .Where(e => e.EmpleadoId == null)
                .ToListAsync();
            var result = envios.Select(e => new {
                e.Id,
                GuiaRastreo = e.GuiaRastreo,
                FechaEntrega = e.FechaEntrega,
                VentaId = e.VentaId
            }).ToList();
            return Ok(result);
        }

        // GET: api/envios/bitacora/{empleadoId}
        [HttpGet("bitacora/{empleadoId}")]
        public async Task<ActionResult<IEnumerable<BitacoraEntregaDto>>> GetBitacoraChofer(int empleadoId)
        {
            var envios = await _context.Envios
                .Include(e => e.Venta)
                    .ThenInclude(v => v.Destinatario)
                .Include(e => e.Seguimientos)
                .Where(e => e.EmpleadoId == empleadoId)
                .ToListAsync();

            var entregas = new List<BitacoraEntregaDto>();
            foreach (var envio in envios)
            {
                var seguimientoEntregado = envio.Seguimientos
                    .Where(s => s.Status == "Entregado" || s.Status == "Incidencia")
                    .OrderByDescending(s => s.FechaStatus)
                    .FirstOrDefault();
                if (seguimientoEntregado == null)
                    continue;

                entregas.Add(new BitacoraEntregaDto
                {
                    Guia = envio.GuiaRastreo,
                    Direccion = envio.Venta?.Destinatario?.Direccion ?? "",
                    Destinatario = envio.Venta?.Destinatario?.Nombre ?? "",
                    Fecha = seguimientoEntregado.FechaStatus,
                    Estado = seguimientoEntregado.Status
                });
            }
            entregas = entregas.OrderByDescending(e => e.Fecha).ToList();
            return Ok(entregas);
        }

        // GET: api/envios/por-venta/{ventaId}
        [HttpGet("por-venta/{ventaId}")]
        public async Task<ActionResult<object>> GetEnvioPorVentaId(int ventaId)
        {
            var envio = await _context.Envios.FirstOrDefaultAsync(e => e.VentaId == ventaId);
            if (envio == null)
                return NotFound();
            return Ok(new { GuiaRastreo = envio.GuiaRastreo });
        }

        public class FechaEntregaRequest
        {
            public DateTime? FechaEntrega { get; set; }
        }

        public class AsignarEnvioRequest
        {
            public string CodigoSeguimiento { get; set; } = string.Empty;
            public int EmpleadoId { get; set; }
            public DateTime? FechaEntrega { get; set; }
            public int VentaId { get; set; }
        }

        public class CambiarEstadoRequest
        {
            public string Status { get; set; } = string.Empty;
        }

        [HttpPost("{envioId}/cambiar-estado")]
        public async Task<IActionResult> CambiarEstado(int envioId, [FromBody] CambiarEstadoRequest req,
            [FromServices] IHubContext<ManyBoxApi.Hubs.BitacoraHub> bitacoraHub,
            [FromServices] IHubContext<ManyBoxApi.Hubs.NotificacionesHub> notificacionesHub,
            [FromServices] IHubContext<ManyBoxApi.Hubs.DashboardHub> dashboardHub)
        {
            if (string.IsNullOrWhiteSpace(req.Status))
                return BadRequest("Status requerido");

            var envio = await _context.Envios
                .Include(e => e.Empleado)
                .Include(e => e.Venta)
                    .ThenInclude(v => v.Destinatario)
                .FirstOrDefaultAsync(e => e.Id == envioId);
            if (envio == null)
                return NotFound();

            var seguimiento = new Models.SeguimientoPaquete
            {
                EnvioId = envioId,
                Status = req.Status,
                FechaStatus = DateTime.Now
            };
            _context.SeguimientoPaquete.Add(seguimiento);
            await _context.SaveChangesAsync();

            if (req.Status.Equals("Entregado", StringComparison.OrdinalIgnoreCase))
            {
                // 1) Notificar bitácora del chofer
                if (envio.EmpleadoId.HasValue)
                {
                    await bitacoraHub.Clients.Group($"bitacora_{envio.EmpleadoId.Value}").SendAsync("BitacoraActualizada");
                }

                // 2) Crear notificación para SuperAdmins y Admin de la sucursal del chofer
                var titulo = "Paquete entregado";
                var msg = $"Guía {envio.GuiaRastreo} entregada a {envio.Venta?.Destinatario?.Nombre ?? "destinatario"}";

                var notificacion = new Notificacion
                {
                    Tipo = "envio",
                    Titulo = titulo,
                    Mensaje = msg,
                    Prioridad = "Media",
                    FechaCreacion = DateTime.UtcNow,
                    Estado = "activa"
                };
                _context.Notificaciones.Add(notificacion);
                await _context.SaveChangesAsync();

                var destinatarios = new List<NotificacionEntrega>();

                // SuperAdmins (rol_id == 1)
                var superAdmins = await _context.Usuarios.Where(u => u.RolId == 1 && u.Activo).Select(u => u.Id).ToListAsync();
                foreach (var uid in superAdmins)
                {
                    _context.NotificacionDestinatarios.Add(new NotificacionDestinatario
                    {
                        NotificacionId = notificacion.Id,
                        UsuarioId = uid,
                        SucursalId = null,
                        RolId = 1
                    });
                    destinatarios.Add(new NotificacionEntrega
                    {
                        NotificacionId = notificacion.Id,
                        UsuarioId = uid,
                        Canal = "ws",
                        Estado = "pendiente",
                        FechaEnvio = DateTime.UtcNow
                    });
                }

                // Admin de la sucursal del chofer (rol_id == 2)
                int? sucChofer = envio.Empleado?.SucursalId;
                if (sucChofer.HasValue)
                {
                    var admins = await _context.Usuarios
                        .Where(u => u.RolId == 2 && u.Activo && u.EmpleadoId != null)
                        .Include(u => u.Empleado)
                        .Where(u => u.Empleado!.SucursalId == sucChofer.Value)
                        .Select(u => u.Id)
                        .ToListAsync();
                    foreach (var uid in admins)
                    {
                        _context.NotificacionDestinatarios.Add(new NotificacionDestinatario
                        {
                            NotificacionId = notificacion.Id,
                            UsuarioId = uid,
                            SucursalId = sucChofer,
                            RolId = 2
                        });
                        destinatarios.Add(new NotificacionEntrega
                        {
                            NotificacionId = notificacion.Id,
                            UsuarioId = uid,
                            Canal = "ws",
                            Estado = "pendiente",
                            FechaEnvio = DateTime.UtcNow
                        });
                    }
                }

                if (destinatarios.Any())
                {
                    _context.NotificacionEntregas.AddRange(destinatarios);
                    await _context.SaveChangesAsync();

                    // Emitir a cada usuario en su grupo
                    foreach (var d in destinatarios)
                    {
                        await notificacionesHub.Clients.Group($"user_{d.UsuarioId}").SendAsync("nuevaNotificacion", new
                        {
                            id = notificacion.Id,
                            titulo,
                            mensaje = msg,
                            fecha = notificacion.FechaCreacion
                        });
                    }
                }

                // 3) Refrescar dashboards conectados
                await dashboardHub.Clients.All.SendAsync("dashboardUpdate");
            }
            else if (req.Status.Equals("Incidencia", StringComparison.OrdinalIgnoreCase))
            {
                if (envio.EmpleadoId.HasValue)
                {
                    await bitacoraHub.Clients.Group($"bitacora_{envio.EmpleadoId.Value}").SendAsync("BitacoraActualizada");
                }
            }

            return Ok();
        }

        public class GuiaChoferDto
        {
            public int EnvioId { get; set; }
            public string GuiaRastreo { get; set; }
            public string Destinatario { get; set; }
            public int VentaId { get; set; }
            public List<EstadoGuiaDto> Estados { get; set; }
            public int EstadoActual { get; set; }
        }

        public class EstadoGuiaDto
        {
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public DateTime FechaHora { get; set; }
        }

        public class BitacoraEntregaDto
        {
            public string Guia { get; set; } = string.Empty;
            public string Direccion { get; set; } = string.Empty;
            public string Destinatario { get; set; } = string.Empty;
            public DateTime Fecha { get; set; }
            public string Estado { get; set; } = string.Empty;
        }
    }
}