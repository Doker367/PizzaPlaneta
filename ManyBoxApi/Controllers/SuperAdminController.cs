using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using ManyBoxApi.Data;

namespace ManyBoxApi.Controllers
{
    [ApiController]
    [Route("api/superadmin")]
    [Authorize] // Restringe si es necesario: Roles = "SuperAdmin"
    public class SuperAdminController : ControllerBase
    {
        private readonly AppDbContext _db;
        public SuperAdminController(AppDbContext db)
        {
            _db = db;
        }

        private static bool ContainsIgnoreCase(string? text, string value)
            => text != null && text.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;

        private async Task<(int totalPaquetes, int enCamino, int entregados, int pendientes)> CalcularResumenPaquetes()
        {
            // Considerar "paquetes" como Ventas (coincide con la grilla de Paquetes)
            var ventasIds = await _db.Ventas.Select(v => v.Id).ToListAsync();
            int totalPaquetes = ventasIds.Count;

            // Mapear Envíos por Venta
            var envios = await _db.Envios
                .Where(e => ventasIds.Contains(e.VentaId))
                .Select(e => new { e.Id, e.VentaId })
                .ToListAsync();
            var envioIds = envios.Select(e => e.Id).ToList();

            // Último seguimiento por Envío
            var ultimos = await _db.SeguimientoPaquete
                .Where(s => envioIds.Contains(s.EnvioId))
                .GroupBy(s => s.EnvioId)
                .Select(g => g.OrderByDescending(s => s.FechaStatus).ThenByDescending(s => s.Id).FirstOrDefault())
                .ToListAsync();

            // Diccionario EnvioId -> Status
            var estadoPorEnvio = ultimos.Where(s => s != null).ToDictionary(s => s!.EnvioId, s => s!.Status);

            // Clasificar por Venta: si no hay Envío o Seguimiento => "Registrado" => pendiente
            int enCamino = 0;
            int entregados = 0;
            foreach (var vId in ventasIds)
            {
                var envio = envios.FirstOrDefault(e => e.VentaId == vId);
                if (envio != null && estadoPorEnvio.TryGetValue(envio.Id, out var st))
                {
                    if (string.Equals(st, "Entregado", StringComparison.OrdinalIgnoreCase))
                    {
                        entregados++;
                    }
                    else if (
                        ContainsIgnoreCase(st, "En camino") ||
                        ContainsIgnoreCase(st, "Último tramo") ||
                        ContainsIgnoreCase(st, "Ultimo tramo") ||
                        ContainsIgnoreCase(st, "Asignado")
                    )
                    {
                        enCamino++;
                    }
                }
            }

            int pendientes = Math.Max(0, totalPaquetes - entregados - enCamino);
            return (totalPaquetes, enCamino, entregados, pendientes);
        }

        // GET: api/superadmin/estadisticas
        [HttpGet("estadisticas")]
        public async Task<ActionResult<EstadisticasDto>> GetEstadisticas()
        {
            var resumen = await CalcularResumenPaquetes();

            // Contar empleados existentes (coincide con tabla empleados)
            int empleadosActivos = await _db.Empleados.CountAsync();

            return Ok(new EstadisticasDto
            {
                paquetesPendientes = resumen.pendientes,
                paquetesEnCamino = resumen.enCamino,
                paquetesEntregados = resumen.entregados,
                empleadosActivos = empleadosActivos
            });
        }

        // ENDPOINTS UNITARIOS
        // GET: api/superadmin/paquetes/pendientes
        [HttpGet("paquetes/pendientes")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<CountDto>> GetPaquetesPendientes()
        {
            var resumen = await CalcularResumenPaquetes();
            return Ok(new CountDto { total = resumen.pendientes });
        }

        // GET: api/superadmin/paquetes/en-camino
        [HttpGet("paquetes/en-camino")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<CountDto>> GetPaquetesEnCamino()
        {
            var resumen = await CalcularResumenPaquetes();
            return Ok(new CountDto { total = resumen.enCamino });
        }

        // GET: api/superadmin/paquetes/entregados
        [HttpGet("paquetes/entregados")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<CountDto>> GetPaquetesEntregados()
        {
            var resumen = await CalcularResumenPaquetes();
            return Ok(new CountDto { total = resumen.entregados });
        }

        // GET: api/superadmin/empleados/activos
        [HttpGet("empleados/activos")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<CountDto>> GetEmpleadosActivos()
        {
            int empleadosActivos = await _db.Empleados.CountAsync();
            return Ok(new CountDto { total = empleadosActivos });
        }

        // GET: api/superadmin/entregas-por-dia?dias=7
        [HttpGet("entregas-por-dia")]
        public async Task<ActionResult<IEnumerable<EntregaDiariaDto>>> GetEntregasPorDia([FromQuery] int dias = 7)
        {
            if (dias <= 0 || dias > 31) dias = 7;
            var desde = DateTime.Today.AddDays(-(dias - 1));

            var entregas = await _db.SeguimientoPaquete
                .Where(s => s.Status == "Entregado" && s.FechaStatus.Date >= desde)
                .GroupBy(s => s.FechaStatus.Date)
                .Select(g => new { Dia = g.Key, Count = g.Count() })
                .ToListAsync();

            var cultura = new CultureInfo("es-ES");
            var list = new List<EntregaDiariaDto>();
            for (int i = 0; i < dias; i++)
            {
                var fecha = desde.AddDays(i).Date;
                var match = entregas.FirstOrDefault(x => x.Dia == fecha);
                list.Add(new EntregaDiariaDto
                {
                    Dia = cultura.DateTimeFormat.GetAbbreviatedDayName(fecha.DayOfWeek).Replace('.', ' ').Trim(),
                    Entregas = match?.Count ?? 0
                });
            }
            return Ok(list);
        }

        // GET: api/superadmin/ultimos-paquetes?take=10
        [HttpGet("ultimos-paquetes")]
        public async Task<ActionResult<IEnumerable<PaqueteResumenDto>>> GetUltimosPaquetes([FromQuery] int take = 10)
        {
            if (take <= 0 || take > 50) take = 10;

            var ventas = await _db.Ventas
                .Include(v => v.Destinatario)
                .OrderByDescending(v => v.Fecha)
                .Take(take)
                .Select(v => new { v.Id, v.Fecha, v.Folio, DestNombre = v.Destinatario != null ? v.Destinatario.Nombre : null })
                .ToListAsync();

            var ventaIds = ventas.Select(v => v.Id).ToList();
            var envios = await _db.Envios
                .Where(e => ventaIds.Contains(e.VentaId))
                .Select(e => new { e.Id, e.VentaId })
                .ToListAsync();
            var envioIds = envios.Select(e => e.Id).ToList();

            var ultimosSegs = await _db.SeguimientoPaquete
                .Where(s => envioIds.Contains(s.EnvioId))
                .GroupBy(s => s.EnvioId)
                .Select(g => g.OrderByDescending(s => s.FechaStatus).ThenByDescending(s => s.Id).FirstOrDefault())
                .ToListAsync();

            var estadoPorEnvio = ultimosSegs.Where(s => s != null).ToDictionary(s => s!.EnvioId, s => s!.Status);

            var list = new List<PaqueteResumenDto>();
            foreach (var v in ventas)
            {
                var envio = envios.FirstOrDefault(e => e.VentaId == v.Id);
                string estado = "Registrado";
                if (envio != null && estadoPorEnvio.TryGetValue(envio.Id, out var st)) estado = st;

                list.Add(new PaqueteResumenDto
                {
                    Id = v.Id,
                    RemitenteNombre = $"Venta {v.Folio}",
                    DestinatarioNombre = v.DestNombre ?? "-",
                    Estado = estado,
                    FechaRegistro = v.Fecha
                });
            }
            return Ok(list);
        }

        // GET: api/superadmin/estadisticas-sucursal?sucursal=Nombre
        [HttpGet("estadisticas-sucursal")]
        public async Task<ActionResult<EstadisticasSucursalDto>> GetEstadisticasSucursal([FromQuery] string sucursal)
        {
            if (string.IsNullOrWhiteSpace(sucursal)) return BadRequest("Debe especificar sucursal");

            var suc = await _db.Sucursales.FirstOrDefaultAsync(s => s.Nombre.ToLower() == sucursal.ToLower());
            if (suc == null) return NotFound("Sucursal no encontrada");

            var desde = DateTime.Today.AddDays(-30);

            int paquetes = await _db.Ventas
                .Join(_db.Empleados, v => v.Empleado_Id, e => e.Id, (v, e) => new { v, e })
                .Where(ve => ve.e.SucursalId == suc.Id && ve.v.Fecha >= desde)
                .CountAsync();

            int empleados = await _db.Empleados
                .Where(e => e.SucursalId == suc.Id)
                .CountAsync();

            int satisfaccion = 95;

            return Ok(new EstadisticasSucursalDto
            {
                paquetes = paquetes,
                empleados = empleados,
                satisfaccion = satisfaccion
            });
        }

        public sealed class EstadisticasDto
        {
            [JsonPropertyName("paquetesPendientes")] public int paquetesPendientes { get; set; }
            [JsonPropertyName("paquetesEnCamino")] public int paquetesEnCamino { get; set; }
            [JsonPropertyName("paquetesEntregados")] public int paquetesEntregados { get; set; }
            [JsonPropertyName("empleadosActivos")] public int empleadosActivos { get; set; }
        }

        public sealed class CountDto
        {
            public int total { get; set; }
        }

        public sealed class EntregaDiariaDto
        {
            public string Dia { get; set; } = string.Empty;
            public int Entregas { get; set; }
        }

        public sealed class PaqueteResumenDto
        {
            public int Id { get; set; }
            public string RemitenteNombre { get; set; } = string.Empty;
            public string DestinatarioNombre { get; set; } = string.Empty;
            public string Estado { get; set; } = string.Empty;
            public DateTime FechaRegistro { get; set; }
        }

        public sealed class EstadisticasSucursalDto
        {
            public int paquetes { get; set; }
            public int empleados { get; set; }
            public int satisfaccion { get; set; }
        }
    }
}
