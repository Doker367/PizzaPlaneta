using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManyBoxApi.Data;
using ManyBoxApi.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;
using ManyBoxApi.Models; // agregado para resolver tipos Venta y Envio

namespace ManyBoxApi.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _db;
        public DashboardController(AppDbContext db)
        {
            _db = db;
        }

        // --- Private Helpers ---
        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }

        private int? GetEmpleadoIdFromClaims()
        {
            var empleadoIdClaim = User.FindFirst("EmpleadoId");
            if (empleadoIdClaim != null && int.TryParse(empleadoIdClaim.Value, out int empleadoId))
            {
                return empleadoId;
            }
            return null;
        }

        private int? GetSucursalIdFromClaims()
        {
            var sucursalIdClaim = User.FindFirst("SucursalId");
            if (sucursalIdClaim != null && int.TryParse(sucursalIdClaim.Value, out int sucursalId))
            {
                return sucursalId;
            }
            return null;
        }

        private IQueryable<Venta> GetVentasQuery()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var empleadoId = GetEmpleadoIdFromClaims();
            var sucursalId = GetSucursalIdFromClaims();

            IQueryable<Venta> query = _db.Ventas;

            if (userRole == "Admin" && sucursalId.HasValue)
            {
                var empleadosEnSucursal = _db.Empleados.Where(e => e.SucursalId == sucursalId.Value).Select(e => e.Id);
                query = query.Where(v => v.Empleado_Id.HasValue && empleadosEnSucursal.Contains(v.Empleado_Id.Value));
            }
            else if (userRole != "SuperAdmin" && empleadoId.HasValue)
            {
                query = query.Where(v => v.Empleado_Id == empleadoId.Value);
            }
            return query;
        }

        private IQueryable<Envio> GetEnviosQuery()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var empleadoId = GetEmpleadoIdFromClaims();
            var sucursalId = GetSucursalIdFromClaims();

            IQueryable<Envio> query = _db.Envios;

            if (userRole == "Admin" && sucursalId.HasValue)
            {
                var empleadosEnSucursal = _db.Empleados.Where(e => e.SucursalId == sucursalId.Value).Select(e => e.Id);
                query = query.Where(e => e.Venta != null && e.Venta.Empleado_Id.HasValue && empleadosEnSucursal.Contains(e.Venta.Empleado_Id.Value));
            }
            else if (userRole != "SuperAdmin" && empleadoId.HasValue)
            {
                query = query.Where(e => e.Venta != null && e.Venta.Empleado_Id == empleadoId.Value);
            }
            return query;
        }

        // --- Endpoints ---

        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetStats()
        {
            var ventasQuery = GetVentasQuery();
            var enviosQuery = GetEnviosQuery();

            var enviosActivos = await enviosQuery.CountAsync(e => e.FechaEntrega == null);
            var totalEnvios = await enviosQuery.CountAsync();
            var entregados = await enviosQuery.CountAsync(e => e.FechaEntrega != null);
            var tasaEntrega = totalEnvios > 0 ? (int)Math.Round((double)entregados / totalEnvios * 100) : 0;

            var satisfaccion = 90; // Simulado

            var desde = DateTime.Today.AddDays(-7);
            var nuevosPaquetes = await ventasQuery.CountAsync(v => v.Fecha >= desde);

            return Ok(new DashboardStatsDto
            {
                EnviosActivos = enviosActivos,
                TasaEntrega = tasaEntrega,
                Satisfaccion = satisfaccion,
                NuevosPaquetes = nuevosPaquetes
            });
        }

        [HttpGet("stats-history")]
        public async Task<IActionResult> GetStatsHistory([FromQuery] string periodo = "dia")
        {
            DateTime fechaInicio;
            DateTime fechaFin = DateTime.Now;

            switch (periodo)
            {
                case "semana": fechaInicio = DateTime.Today.AddDays(-7); break;
                case "mes": fechaInicio = DateTime.Today.AddMonths(-1); break;
                default: fechaInicio = DateTime.Today.AddDays(-1); fechaFin = DateTime.Today; break;
            }

            var ventasQuery = GetVentasQuery().Where(v => v.Fecha >= fechaInicio && v.Fecha < fechaFin);
            var enviosQuery = GetEnviosQuery().Where(e => e.Venta != null && e.Venta.Fecha >= fechaInicio && e.Venta.Fecha < fechaFin);

            var enviosActivos = await enviosQuery.CountAsync(e => e.FechaEntrega == null);
            var totalEnvios = await enviosQuery.CountAsync();
            var entregados = await enviosQuery.CountAsync(e => e.FechaEntrega != null);
            var tasaEntrega = totalEnvios > 0 ? (int)(((double)entregados / totalEnvios) * 100) : 0;
            var satisfaccion = 98; // Simulado
            var nuevosPaquetes = await ventasQuery.CountAsync();

            return Ok(new DashboardStatsHistoryDto
            {
                EnviosActivos = enviosActivos,
                TasaEntrega = tasaEntrega,
                Satisfaccion = satisfaccion,
                NuevosPaquetes = nuevosPaquetes,
                Fecha = fechaInicio
            });
        }

        [HttpGet("entregas-mensuales")]
        public async Task<ActionResult<IEnumerable<EntregasMensualesDto>>> GetEntregasMensuales()
        {
            var enviosQuery = GetEnviosQuery();
            var haceUnAnio = DateTime.Now.AddYears(-1);

            var data = await enviosQuery
                .Where(e => e.Venta.Fecha >= haceUnAnio)
                .GroupBy(e => e.Venta.Fecha.Month)
                .Select(g => new EntregasMensualesDto
                {
                    Mes = g.Key,
                    Entregados = g.Count(e => e.FechaEntrega != null),
                    Pendientes = g.Count(e => e.FechaEntrega == null)
                })
                .OrderBy(r => r.Mes)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("actividad-reciente")]
        public async Task<ActionResult<IEnumerable<ActividadRecienteDto>>> GetActividadReciente()
        {
            var ventasQuery = GetVentasQuery();
            var actividades = await ventasQuery
                .OrderByDescending(v => v.Fecha)
                .Take(5)
                .Select(v => new ActividadRecienteDto
                {
                    Titulo = "Nueva Venta",
                    Descripcion = $"Venta con folio {v.Folio} registrada.",
                    Fecha = v.Fecha,
                    Icono = "fas fa-receipt"
                })
                .ToListAsync();

            return Ok(actividades);
        }

        [HttpGet("notificaciones")]
        public async Task<ActionResult<IEnumerable<NotificacionDto>>> GetNotificaciones()
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();

            var notificaciones = await _db.NotificacionDestinatarios
                .Where(nd => nd.UsuarioId == userId.Value)
                .OrderByDescending(nd => nd.Notificacion.FechaCreacion)
                .Take(10)
                .Select(nd => new NotificacionDto
                {
                    Titulo = nd.Notificacion.Titulo,
                    Mensaje = nd.Notificacion.Mensaje,
                    Fecha = nd.Notificacion.FechaCreacion,
                    Prioridad = nd.Notificacion.Prioridad
                })
                .ToListAsync();

            return Ok(notificaciones);
        }

        [HttpGet("rendimiento-sucursales")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<IEnumerable<SucursalRendimientoDto>>> GetRendimientoSucursales()
        {
            var totalVentas = await _db.Ventas.CountAsync();
            if (totalVentas == 0) return Ok(new List<SucursalRendimientoDto>());

            var rendimiento = await _db.Ventas
                .Where(v => v.Empleado_Id != null)
                .Join(_db.Empleados.Include(e => e.Sucursal),
                      v => v.Empleado_Id,
                      e => e.Id,
                      (v, e) => new { v, e })
                .Where(joined => joined.e.SucursalId != null)
                .GroupBy(joined => joined.e.Sucursal.Nombre)
                .Select(g => new SucursalRendimientoDto
                {
                    Nombre = g.Key,
                    Porcentaje = (int)Math.Round((double)g.Count() / totalVentas * 100)
                })
                .OrderByDescending(r => r.Porcentaje)
                .ToListAsync();

            return Ok(rendimiento);
        }
    }
}