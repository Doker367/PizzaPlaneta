using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManyBoxApi.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ManyBoxApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class EmpleadosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EmpleadosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/empleados?rol=chofer&skip=0&take=12
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmpleadoVM>>> GetEmpleados([FromQuery] string rol = null, [FromQuery] int skip = 0, [FromQuery] int take = 12)
        {
            var query = _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Empleado)
                .Where(u => u.Empleado != null);

            if (!string.IsNullOrEmpty(rol))
            {
                query = query.Where(u => u.Rol != null && u.Rol.Nombre.ToLower().Contains(rol.ToLower()));
            }

            var empleados = await query
                .OrderBy(u => u.Empleado.Nombre)
                .Skip(skip)
                .Take(take)
                .Select(u => new EmpleadoVM
                {
                    Id = u.Empleado.Id,
                    Nombre = u.Empleado.Nombre,
                    Apellido = u.Empleado.Apellido,
                    NombreCompleto = u.Empleado.Nombre + " " + u.Empleado.Apellido
                })
                .ToListAsync();

            return Ok(empleados);
        }

        public class EmpleadoVM
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string NombreCompleto { get; set; }
        }
    }
}
