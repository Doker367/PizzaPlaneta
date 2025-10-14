using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManyBoxApi.Data;
using ManyBoxApi.DTOs;
using System.Data.Common;

namespace ManyBoxApi.Controllers
{
    [ApiController]
    [Route("api/ventas-estado")] // Nuevo endpoint dedicado para lista de ventas con estado real
    public class VentasEstadoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public VentasEstadoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ventas-estado/filtrados?usuarioId=..&rol=..
        // Esta acción siempre toma el estado desde la columna ventas.completada (tabla ventas)
        [HttpGet("filtrados")]
        public async Task<ActionResult<IEnumerable<VentaDto>>> GetFiltrados([FromQuery] int? usuarioId = null, [FromQuery] string? rol = null)
        {
            var ventasQuery = _context.Ventas
                .Include(v => v.Remitente)
                .Include(v => v.Destinatario)
                .Include(v => v.DetalleContenido)
                .AsNoTracking()
                .AsQueryable();

            // Resolver datos del usuario para filtrar por rol
            int? empleadoIdActual = null;
            int? sucursalIdActual = null;
            if (usuarioId.HasValue)
            {
                var usuarioDb = await _context.Usuarios
                    .Include(u => u.Empleado)
                    .FirstOrDefaultAsync(u => u.Id == usuarioId.Value);
                empleadoIdActual = usuarioDb?.EmpleadoId;
                sucursalIdActual = usuarioDb?.Empleado?.SucursalId;
            }

            var rolNorm = (rol ?? string.Empty).Trim().ToLowerInvariant();
            // Filtros:
            // - superadmin: todas
            // - admin: solo ventas de su sucursal
            // - empleado: solo ventas hechas por él

            if (rolNorm == "empleado")
            {
                if (empleadoIdActual.HasValue)
                    ventasQuery = ventasQuery.Where(v => v.Empleado_Id == empleadoIdActual.Value);
                else
                    ventasQuery = ventasQuery.Where(v => false); // sin contexto => nada
            }
            else if (rolNorm == "admin")
            {
                if (sucursalIdActual.HasValue)
                {
                    // Obtener ids de empleados de la sucursal actual y filtrar ventas por esos empleados
                    var empIds = await _context.Empleados
                        .Where(e => e.SucursalId == sucursalIdActual.Value)
                        .Select(e => e.Id)
                        .ToListAsync();
                    ventasQuery = ventasQuery.Where(v => v.Empleado_Id != null && empIds.Contains(v.Empleado_Id.Value));
                }
                else
                {
                    ventasQuery = ventasQuery.Where(v => false);
                }
            }
            else if (!string.IsNullOrEmpty(rolNorm) && rolNorm != "superadmin")
            {
                // Rol desconocido: no exponer datos
                ventasQuery = ventasQuery.Where(v => false);
            }

            var empleados = await _context.Empleados
                .Include(e => e.Sucursal)
                .AsNoTracking()
                .ToListAsync();

            var ventas = await ventasQuery.OrderByDescending(v => v.Fecha).ToListAsync();

            // Leer 'completada' directamente de la tabla ventas para asegurar fuente
            var estadoPorId = await LeerCompletadaPorIdsAsync(ventas.Select(v => v.Id));

            var result = ventas.Select(venta => new VentaDto
            {
                Id = venta.Id,
                Fecha = venta.Fecha,
                Folio = venta.Folio,
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
                DetalleContenido = venta.DetalleContenido?.Select(d => new DetalleContenidoDto
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
                SucursalOrigen = venta.Empleado_Id != null ? empleados.FirstOrDefault(e => e.Id == venta.Empleado_Id)?.Sucursal?.Nombre : null,
                DestinatarioNombre = venta.Destinatario?.Nombre,
                Completada = estadoPorId.TryGetValue(venta.Id, out var flag) && flag
            }).ToList();

            return Ok(result);
        }

        private async Task<Dictionary<int, bool>> LeerCompletadaPorIdsAsync(IEnumerable<int> idsEnum)
        {
            var ids = idsEnum.Distinct().ToList();
            var dict = new Dictionary<int, bool>();
            if (ids.Count == 0) return dict;

            var inList = string.Join(",", ids);
            var sql = $"SELECT id, completada FROM ventas WHERE id IN ({inList})";
            await using var conn = _context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var id = reader.GetInt32(0);
                // MySQL puede devolver tinyint(1) como sbyte/byte/bool; maneja todo
                bool completed;
                var value = reader.GetValue(1);
                if (value is bool b) completed = b;
                else if (value is byte by) completed = by != 0;
                else if (value is sbyte sb) completed = sb != 0;
                else completed = Convert.ToInt32(value) != 0;
                dict[id] = completed;
            }
            return dict;
        }
    }
}
