using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManyBoxApi.Data;
using ManyBoxApi.DTOs;
using ManyBoxApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ManyBoxApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        // Nuevo generador de folio: S + sucursalId(0-izq si <10) + yyyyMMdd + ventaId
        private static string BuildFolio(int sucursalId, DateTime fecha, int ventaId)
        {
            var sucStr = sucursalId < 10 ? $"0{sucursalId}" : sucursalId.ToString();
            var fechaStr = fecha.ToString("yyyyMMdd");
            var idStr = ventaId.ToString("D2"); // mínimo 2 dígitos, sin límite superior
            return $"S{sucStr}{fechaStr}{idStr}";
        }

        // GET: api/ventas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VentaDto>>> GetVentas([FromQuery] string rol = null, [FromQuery] int? empleadoId = null, [FromQuery] int? sucursalId = null)
        {
            var ventasQuery = _context.Ventas
                .Include(v => v.Remitente)
                .Include(v => v.Destinatario)
                .Include(v => v.DetalleContenido)
                .Include(v => v.Envios)
                .AsQueryable();

            var empleados = await _context.Empleados.Include(e => e.Sucursal).ToListAsync();
            var clientes = await _context.Clientes.ToListAsync();

            if (!string.IsNullOrEmpty(rol))
            {
                rol = rol.ToLower();
                if (rol == "empleado" && empleadoId.HasValue)
                {
                    ventasQuery = ventasQuery.Where(v => v.Empleado_Id == empleadoId.Value);
                }
                else if (rol == "admin" && sucursalId.HasValue)
                {
                    var empleadosSucursal = empleados.Where(e => e.SucursalId == sucursalId.Value).Select(e => e.Id).ToList();
                    ventasQuery = ventasQuery.Where(v => v.Empleado_Id != null && empleadosSucursal.Contains(v.Empleado_Id.Value));
                }
            }

            var ventas = await ventasQuery.ToListAsync();

            var result = ventas.Select(v =>
            {
                var empleadoVenta = v.Empleado_Id != null ? empleados.FirstOrDefault(e => e.Id == v.Empleado_Id) : null;
                var sucId = empleadoVenta?.SucursalId ?? 0;
                var folioMostrar = string.IsNullOrWhiteSpace(v.Folio) ? BuildFolio(sucId, v.Fecha.Date, v.Id) : v.Folio;
                return new VentaDto
                {
                    Id = v.Id,
                    Fecha = v.Fecha,
                    Folio = folioMostrar,
                    Valor_Declarado = v.Valor_Declarado,
                    Medidas = v.Medidas,
                    Peso_Volumetrico = v.Peso_Volumetrico,
                    Peso_Fisico = v.Peso_Fisico,
                    Seguro = v.Seguro,
                    Compania_Envio = v.Compania_Envio,
                    Tipo_Riesgo = v.Tipo_Riesgo,
                    Tipo_Pago = v.Tipo_Pago,
                    Costo_Envio = v.Costo_Envio,
                    Total_Piezas = v.Total_Piezas,
                    Tiempo_Estimado = v.Tiempo_Estimado,
                    Total_Cobrado = v.Total_Cobrado,
                    Remitente = v.Remitente == null ? null : new RemitenteDto
                    {
                        Nombre = v.Remitente.Nombre,
                        Telefono = v.Remitente.Telefono,
                        Compania = v.Remitente.Compania,
                        Direccion = v.Remitente.Direccion,
                        Ciudad = v.Remitente.Ciudad,
                        Estado = v.Remitente.Estado,
                        Pais = v.Remitente.Pais,
                        CP = v.Remitente.CP
                    },
                    Destinatario = v.Destinatario == null ? null : new DestinatarioDto
                    {
                        Nombre = v.Destinatario.Nombre,
                        Telefono = v.Destinatario.Telefono,
                        Compania = v.Destinatario.Compania,
                        Direccion = v.Destinatario.Direccion,
                        Ciudad = v.Destinatario.Ciudad,
                        Estado = v.Destinatario.Estado,
                        Pais = v.Destinatario.Pais,
                        CP = v.Destinatario.CP
                    },
                    DetalleContenido = v.DetalleContenido.Select(d => new DetalleContenidoDto
                    {
                        Descripcion = d.Descripcion,
                        Cantidad = d.Cantidad,
                        Unidad = d.Unidad
                    }).ToList(),
                    Empleado = v.Empleado_Id != null ? empleados.Where(e => e.Id == v.Empleado_Id).Select(e => new EmpleadoDto
                    {
                        Id = e.Id,
                        Nombre = e.Nombre,
                        Apellido = e.Apellido
                    }).FirstOrDefault() : null,
                    SucursalOrigen = empleadoVenta?.Sucursal?.Nombre,
                    DestinatarioNombre = v.Destinatario?.Nombre,
                    Completada = v.Completada,
                    ClienteId = v.Cliente_Id,
                    ClienteNombre = clientes.FirstOrDefault(c => c.Id == v.Cliente_Id)?.Nombre
                };
            }).ToList();

            return Ok(result);
        }

        // GET: api/ventas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<VentaDto>> GetVentaPorId(int id)
        {
            var empleados = await _context.Empleados.Include(e => e.Sucursal).ToListAsync();
            var clientes = await _context.Clientes.ToListAsync();

            var venta = await _context.Ventas
                .Include(v => v.Remitente)
                .Include(v => v.Destinatario)
                .Include(v => v.DetalleContenido)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
            {
                return NotFound();
            }

            var empleadoVenta = venta.Empleado_Id != null ? empleados.FirstOrDefault(e => e.Id == venta.Empleado_Id) : null;
            var sucId = empleadoVenta?.SucursalId ?? 0;
            var folioMostrar = string.IsNullOrWhiteSpace(venta.Folio) ? BuildFolio(sucId, venta.Fecha.Date, venta.Id) : venta.Folio;

            var result = new VentaDto
            {
                Id = venta.Id,
                Fecha = venta.Fecha,
                Folio = folioMostrar,
                Valor_Declarado = venta.Valor_Declarado,
                Medidas = venta.Medidas,
                Peso_Volumetrico = venta.Peso_Volumetrico,
                Peso_Fisico = venta.Peso_Fisico,
                Seguro = venta.Seguro,
                Compania_Envio = venta.Compania_Envio,
                Tipo_Riesgo = venta.Tipo_Riesgo,
                Tipo_Pago = venta.Tipo_Pago,
                Costo_Envio = venta.Costo_Envio,
                Total_Piezas = venta.Total_Piezas,
                Tiempo_Estimado = venta.Tiempo_Estimado,
                Total_Cobrado = venta.Total_Cobrado,
                Remitente = venta.Remitente == null ? null : new RemitenteDto
                {
                    Nombre = venta.Remitente.Nombre,
                    Telefono = venta.Remitente.Telefono,
                    Compania = venta.Remitente.Compania,
                    Direccion = venta.Remitente.Direccion,
                    Ciudad = venta.Remitente.Ciudad,
                    Estado = venta.Remitente.Estado,
                    Pais = venta.Remitente.Pais,
                    CP = venta.Remitente.CP
                },
                Destinatario = venta.Destinatario == null ? null : new DestinatarioDto
                {
                    Nombre = venta.Destinatario.Nombre,
                    Telefono = venta.Destinatario.Telefono,
                    Compania = venta.Destinatario.Compania,
                    Direccion = venta.Destinatario.Direccion,
                    Ciudad = venta.Destinatario.Ciudad,
                    Estado = venta.Destinatario.Estado,
                    Pais = venta.Destinatario.Pais,
                    CP = venta.Destinatario.CP
                },
                DetalleContenido = venta.DetalleContenido.Select(d => new DetalleContenidoDto
                {
                    Descripcion = d.Descripcion,
                    Cantidad = d.Cantidad,
                    Unidad = d.Unidad
                }).ToList(),
                Empleado = venta.Empleado_Id != null ? empleados.Where(e => e.Id == venta.Empleado_Id).Select(e => new EmpleadoDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Apellido = e.Apellido
                }).FirstOrDefault() : null,
                SucursalOrigen = empleadoVenta?.Sucursal?.Nombre,
                DestinatarioNombre = venta.Destinatario?.Nombre,
                Completada = venta.Completada,
                ClienteId = venta.Cliente_Id,
                ClienteNombre = clientes.FirstOrDefault(c => c.Id == venta.Cliente_Id)?.Nombre
            };

            return Ok(result);
        }

        // POST: api/ventas
        [HttpPost]
        public async Task<ActionResult<Venta>> CrearVenta([FromBody] CrearVentaRequest request,
            [FromServices] IHubContext<ManyBoxApi.Hubs.NotificacionesHub> notificacionesHub,
            [FromServices] IHubContext<ManyBoxApi.Hubs.DashboardHub> dashboardHub)
        {
            if (request.Cliente_Id == null)
            {
                return BadRequest("El campo Cliente_Id es obligatorio.");
            }

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var usuario = await _context.Usuarios.Include(u => u.Rol).FirstOrDefaultAsync(u => u.Id == userId);
            if (usuario == null || usuario.EmpleadoId == null)
                return BadRequest("El usuario autenticado no tiene empleado vinculado.");

            var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.Id == usuario.EmpleadoId);
            int sucursalId = empleado?.SucursalId ?? 0;

            var rolNombre = usuario.Rol?.Nombre ?? string.Empty;
            bool esSuperAdmin = string.Equals(rolNombre, "SuperAdmin", StringComparison.OrdinalIgnoreCase);
            bool esAdmin = string.Equals(rolNombre, "Admin", StringComparison.OrdinalIgnoreCase);
            bool esEmpleado = string.Equals(rolNombre, "Empleado", StringComparison.OrdinalIgnoreCase);

            var remitente = new Remitente
            {
                Nombre = request.Remitente_Nombre,
                Telefono = request.Remitente_Telefono,
                Compania = request.Remitente_Compania,
                Direccion = request.Remitente_Direccion,
                Ciudad = request.Remitente_Ciudad,
                Estado = request.Remitente_Estado,
                Pais = request.Remitente_Pais,
                CP = request.Remitente_Cp
            };
            _context.Add(remitente);
            await _context.SaveChangesAsync();

            var destinatario = new Destinatario
            {
                Nombre = request.Destinatario_Nombre,
                Telefono = request.Destinatario_Telefono,
                Compania = request.Destinatario_Compania,
                Direccion = request.Destinatario_Direccion,
                Ciudad = request.Destinatario_Ciudad,
                Estado = request.Destinatario_Estado,
                Pais = request.Destinatario_Pais,
                CP = request.Destinatario_Cp
            };
            _context.Add(destinatario);
            await _context.SaveChangesAsync();

            // Obtener el próximo AUTO_INCREMENT para construir el folio antes del insert
            int nextId = 0;
            await using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                await _context.Database.OpenConnectionAsync();
                cmd.CommandText = "SELECT AUTO_INCREMENT FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = 'ventas'";
                var scalar = await cmd.ExecuteScalarAsync();
                _ = int.TryParse(Convert.ToString(scalar), out nextId);
                await _context.Database.CloseConnectionAsync();
            }

            var folioCalculado = BuildFolio(sucursalId, request.Fecha.Date, nextId);
            var folioFinal = (esSuperAdmin && !string.IsNullOrWhiteSpace(request.Folio)) ? request.Folio : folioCalculado;

            var venta = new Venta
            {
                Fecha = request.Fecha,
                Folio = folioFinal, // ya no NULL
                Cliente_Id = request.Cliente_Id ?? 0,
                Empleado_Id = usuario.EmpleadoId.Value,
                Remitente_Id = remitente.Id,
                Destinatario_Id = destinatario.Id,
                Valor_Declarado = request.Valor_Declarado ?? 0,
                Medidas = request.Medidas,
                Peso_Volumetrico = request.Peso_Volumetrico ?? 0,
                Peso_Fisico = request.Peso_Fisico ?? 0,
                Seguro = request.Seguro ?? false,
                Compania_Envio = request.Compania_Envio,
                Tipo_Riesgo = request.Tipo_Riesgo,
                Tipo_Pago = request.Tipo_Pago,
                Costo_Envio = request.Costo_Envio ?? 0,
                Total_Piezas = request.Total_Piezas ?? 0,
                Tiempo_Estimado = request.Tiempo_Estimado,
                Total_Cobrado = request.Total_Cobrado ?? 0,
                Completada = request.Completada
            };

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            if (request.DetalleContenido != null && request.DetalleContenido.Count > 0)
            {
                foreach (var det in request.DetalleContenido)
                {
                    _context.DetalleContenidos.Add(new DetalleContenido
                    {
                        VentaId = venta.Id,
                        Descripcion = det.Descripcion,
                        Cantidad = det.Cantidad,
                        Unidad = det.Unidad
                    });
                }
                await _context.SaveChangesAsync();
            }

            // Notificación según rol
            try
            {
                var titulo = "Venta registrada";
                var msg = $"Folio {venta.Folio} registrado por {rolNombre}.";

                var notificacion = new Notificacion
                {
                    Tipo = "venta",
                    Titulo = titulo,
                    Mensaje = msg,
                    Prioridad = "Media",
                    FechaCreacion = DateTime.UtcNow,
                    Estado = "activa"
                };
                _context.Notificaciones.Add(notificacion);
                await _context.SaveChangesAsync();

                var entregas = new List<NotificacionEntrega>();

                if (esEmpleado)
                {
                    // Enviar a Admins de la sucursal del empleado
                    var admins = await _context.Usuarios
                        .Where(u => u.RolId == 2 && u.Activo && u.EmpleadoId != null)
                        .Include(u => u.Empleado)
                        .Where(u => u.Empleado!.SucursalId == sucursalId)
                        .Select(u => u.Id)
                        .ToListAsync();

                    foreach (var uid in admins)
                    {
                        _context.NotificacionDestinatarios.Add(new NotificacionDestinatario
                        {
                            NotificacionId = notificacion.Id,
                            UsuarioId = uid,
                            SucursalId = sucursalId,
                            RolId = 2
                        });
                        entregas.Add(new NotificacionEntrega
                        {
                            NotificacionId = notificacion.Id,
                            UsuarioId = uid,
                            Canal = "ws",
                            Estado = "pendiente",
                            FechaEnvio = DateTime.UtcNow
                        });
                    }
                }
                else if (esAdmin)
                {
                    // Enviar a SuperAdmins
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
                        entregas.Add(new NotificacionEntrega
                        {
                            NotificacionId = notificacion.Id,
                            UsuarioId = uid,
                            Canal = "ws",
                            Estado = "pendiente",
                            FechaEnvio = DateTime.UtcNow
                        });
                    }
                }

                if (entregas.Count > 0)
                {
                    _context.NotificacionEntregas.AddRange(entregas);
                    await _context.SaveChangesAsync();

                    // Emitir a cada usuario
                    foreach (var d in entregas)
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

                // Refrescar dashboard (tarjetas de nuevos paquetes/ventas)
                await dashboardHub.Clients.All.SendAsync("dashboardUpdate");
            }
            catch
            {
                // No interrumpir el flujo de creación si falla la notificación
            }

            return CreatedAtAction(nameof(GetVentaPorId), new { id = venta.Id }, venta);
        }

        // PUT: api/ventas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarVenta(int id, [FromBody] CrearVentaRequest request)
        {
            var venta = await _context.Ventas.Include(v => v.Remitente).Include(v => v.Destinatario).FirstOrDefaultAsync(v => v.Id == id);
            if (venta == null) return NotFound();

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == userId);
            var rolNombre = await _context.Roles.Where(r => r.Id == usuario.RolId).Select(r => r.Nombre).FirstOrDefaultAsync();
            bool esSuperAdmin = string.Equals(rolNombre, "SuperAdmin", StringComparison.OrdinalIgnoreCase);

            // Solo SuperAdmin puede cambiar folio manualmente. Para otros, se mantiene el existente o se calcula si faltara.
            if (esSuperAdmin && !string.IsNullOrWhiteSpace(request.Folio))
            {
                venta.Folio = request.Folio;
            }
            else if (string.IsNullOrWhiteSpace(venta.Folio))
            {
                // calcular si no tuviera folio
                var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.Id == venta.Empleado_Id);
                var sucId = empleado?.SucursalId ?? 0;
                venta.Folio = BuildFolio(sucId, request.Fecha.Date, venta.Id);
            }

            venta.Fecha = request.Fecha;
            venta.Cliente_Id = request.Cliente_Id ?? venta.Cliente_Id;
            venta.Empleado_Id = venta.Empleado_Id;
            venta.Valor_Declarado = request.Valor_Declarado ?? venta.Valor_Declarado;
            venta.Medidas = request.Medidas;
            venta.Peso_Volumetrico = request.Peso_Volumetrico ?? venta.Peso_Volumetrico;
            venta.Peso_Fisico = request.Peso_Fisico ?? venta.Peso_Fisico;
            venta.Seguro = request.Seguro ?? venta.Seguro;
            venta.Compania_Envio = request.Compania_Envio;
            venta.Tipo_Riesgo = request.Tipo_Riesgo;
            venta.Tipo_Pago = request.Tipo_Pago;
            venta.Costo_Envio = request.Costo_Envio ?? venta.Costo_Envio;
            venta.Total_Piezas = request.Total_Piezas ?? venta.Total_Piezas;
            venta.Tiempo_Estimado = request.Tiempo_Estimado;
            venta.Total_Cobrado = request.Total_Cobrado ?? venta.Total_Cobrado;
            venta.Completada = request.Completada;

            if (venta.Remitente != null)
            {
                venta.Remitente.Nombre = request.Remitente_Nombre;
                venta.Remitente.Telefono = request.Remitente_Telefono;
                venta.Remitente.Compania = request.Remitente_Compania;
                venta.Remitente.Direccion = request.Remitente_Direccion;
                venta.Remitente.Ciudad = request.Remitente_Ciudad;
                venta.Remitente.Estado = request.Remitente_Estado;
                venta.Remitente.Pais = request.Remitente_Pais;
                venta.Remitente.CP = request.Remitente_Cp;
            }
            if (venta.Destinatario != null)
            {
                venta.Destinatario.Nombre = request.Destinatario_Nombre;
                venta.Destinatario.Telefono = request.Destinatario_Telefono;
                venta.Destinatario.Compania = request.Destinatario_Compania;
                venta.Destinatario.Direccion = request.Destinatario_Direccion;
                venta.Destinatario.Ciudad = request.Destinatario_Ciudad;
                venta.Destinatario.Estado = request.Destinatario_Estado;
                venta.Destinatario.Pais = request.Destinatario_Pais;
                venta.Destinatario.CP = request.Destinatario_Cp;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}