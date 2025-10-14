using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManyBoxApi.Data;
using ManyBoxApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManyBoxApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SucursalesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SucursalesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Sucursales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetSucursales()
        {
            var sucursales = await _context.Sucursales
                .Select(s => new { s.Id, s.Nombre, Direccion = s.SucursalDireccion })
                .ToListAsync();
            return Ok(sucursales);
        }

        // GET: api/Sucursales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetSucursal(int id)
        {
            var s = await _context.Sucursales.FindAsync(id);
            if (s == null) return NotFound();
            return Ok(new { s.Id, s.Nombre, Direccion = s.SucursalDireccion });
        }

        // GET: api/Sucursales/filtrar?nombre=xxx
        [HttpGet("filtrar")]
        public async Task<ActionResult<IEnumerable<object>>> FiltrarSucursales([FromQuery] string? nombre = null)
        {
            var query = _context.Sucursales.AsQueryable();
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                query = query.Where(s => s.Nombre.ToLower().Contains(nombre.ToLower()));
            }
            var sucursales = await query.Select(s => new { s.Id, s.Nombre, Direccion = s.SucursalDireccion }).ToListAsync();
            return Ok(sucursales);
        }

        // POST: api/Sucursales
        [HttpPost]
        public async Task<ActionResult<Sucursal>> PostSucursal([FromBody] Sucursal sucursal)
        {
            // La propiedad "Direccion" del cliente llega como SucursalDireccion por el JsonPropertyName en el modelo
            _context.Sucursales.Add(sucursal);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSucursal), new { id = sucursal.Id }, new { sucursal.Id, sucursal.Nombre, Direccion = sucursal.SucursalDireccion });
        }

        // PUT: api/Sucursales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSucursal(int id, [FromBody] Sucursal sucursal)
        {
            if (id != sucursal.Id)
            {
                return BadRequest();
            }

            _context.Entry(sucursal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SucursalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Sucursales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSucursal(int id)
        {
            var sucursal = await _context.Sucursales.FindAsync(id);
            if (sucursal == null)
            {
                return NotFound();
            }

            _context.Sucursales.Remove(sucursal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Sucursales/nombres
        [HttpGet("nombres")]
        public async Task<ActionResult<IEnumerable<string>>> GetNombresSucursales()
        {
            var nombres = await _context.Sucursales
                .Select(s => s.Nombre)
                .Distinct()
                .ToListAsync();
            return Ok(nombres);
        }

        private bool SucursalExists(int id)
        {
            return _context.Sucursales.Any(e => e.Id == id);
        }
    }
}