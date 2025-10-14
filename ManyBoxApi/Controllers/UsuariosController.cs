using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManyBoxApi.Data;
using ManyBoxApi.Models;
using ManyBoxApi.Helpers;
using ManyBoxApi.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System;

namespace ManyBoxApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult<IEnumerable<UsuarioListVM>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios.Include(u => u.Rol)
                .Select(u => new UsuarioListVM
                {
                    Id = u.Id,
                    Username = u.Username,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    RolNombre = u.Rol.Nombre,
                    Activo = u.Activo
                }).ToListAsync();
            return Ok(usuarios);
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult<UsuarioEmpleadoDetalleVM>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Empleado)
                .ThenInclude(e => e.Sucursal)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)
                return NotFound();

            var detalle = new UsuarioEmpleadoDetalleVM
            {
                UsuarioId = usuario.Id,
                Username = usuario.Username,
                UsuarioNombre = usuario.Nombre,
                UsuarioApellido = usuario.Apellido,
                NombreRol = usuario.Rol?.Nombre ?? string.Empty,
                Activo = usuario.Activo,
                EmpleadoId = usuario.Empleado?.Id,
                EmpleadoNombre = usuario.Empleado?.Nombre,
                EmpleadoApellido = usuario.Empleado?.Apellido,
                FechaNacimiento = usuario.Empleado?.FechaNacimiento,
                Correo = usuario.Empleado?.Correo,
                Telefono = usuario.Empleado?.Telefono,
                SucursalId = usuario.Empleado?.SucursalId,
                SucursalNombre = usuario.Empleado?.Sucursal?.Nombre
            };
            return Ok(detalle);
        }

        // POST: api/Usuarios (Sólo SuperAdmin y Admin)
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult<Usuario>> CreateUsuario([FromBody] CreateUsuarioEmpleadoVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.Password != model.ConfirmPassword)
                return BadRequest(new { message = "Las contraseñas no coinciden." });

            if (await _context.Usuarios.AnyAsync(u => u.Username == model.Username))
                return Conflict(new { message = "El nombre de usuario ya existe." });

            if (await _context.Empleados.AnyAsync(e => e.Correo == model.Correo))
                return Conflict(new { message = "El correo electrónico ya está en uso por otro empleado." });

            // Permitir registrar por nombre de sucursal
            int sucursalId = model.SucursalId;
            if (sucursalId == 0 && !string.IsNullOrWhiteSpace(model.SucursalNombre))
            {
                var sucursal = await _context.Sucursales.FirstOrDefaultAsync(s => s.Nombre.ToLower() == model.SucursalNombre.ToLower());
                if (sucursal == null)
                    return BadRequest(new { message = $"La sucursal con nombre '{model.SucursalNombre}' no existe." });
                sucursalId = sucursal.Id;
            }

            if (!await _context.Sucursales.AnyAsync(s => s.Id == sucursalId))
                return BadRequest(new { message = $"La sucursal con ID {sucursalId} no existe." });

            if (!await _context.Roles.AnyAsync(r => r.Id == model.RolId))
                return BadRequest(new { message = $"El rol con ID {model.RolId} no existe." });

            // RESTRICCIÓN: Si es Admin, solo puede registrar empleados en su sucursal
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (currentUserRole == "Admin")
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var adminSucursalId = await (from u in _context.Usuarios
                                             join e in _context.Empleados on u.EmpleadoId equals e.Id
                                             where u.Id == userId
                                             select e.SucursalId).FirstOrDefaultAsync();
                if (adminSucursalId == 0 || sucursalId != adminSucursalId)
                    return BadRequest(new { message = "Solo puedes registrar empleados en tu propia sucursal." });
            }

            var empleado = new Empleado
            {
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                FechaNacimiento = model.FechaNacimiento,
                Correo = model.Correo,
                Telefono = model.Telefono,
                SucursalId = sucursalId
            };

            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();

            var usuario = new Usuario
            {
                Username = model.Username,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                PasswordHash = PasswordHelper.CreateHash(model.Password),
                RolId = model.RolId,
                EmpleadoId = empleado.Id,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> PutUsuario(int id, [FromBody] UsuarioUpdateVM model)
        {
            if (id != model.Id)
                return BadRequest("El ID de la ruta no coincide con el ID del cuerpo.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuarioDb = await _context.Usuarios.Include(u => u.Empleado).FirstOrDefaultAsync(u => u.Id == id);
            if (usuarioDb == null)
                return NotFound();

            // Actualiza datos de Usuario
            usuarioDb.Nombre = model.Nombre;
            usuarioDb.Apellido = model.Apellido;
            if (model.RolId.HasValue && model.RolId != usuarioDb.RolId)
            {
                var roleExists = await _context.Roles.AnyAsync(r => r.Id == model.RolId.Value);
                if (!roleExists)
                    return BadRequest(new { message = $"El RolId {model.RolId.Value} no existe." });
                usuarioDb.RolId = model.RolId.Value;
            }
            if (model.Activo.HasValue)
                usuarioDb.Activo = model.Activo.Value;

            _context.Entry(usuarioDb).State = EntityState.Modified;

            // Actualiza datos de Empleado si existe
            if (usuarioDb.Empleado != null)
            {
                usuarioDb.Empleado.Nombre = model.Nombre;
                usuarioDb.Empleado.Apellido = model.Apellido;
                usuarioDb.Empleado.Correo = model.Email;
                usuarioDb.Empleado.Telefono = model.Telefono;
                usuarioDb.Empleado.FechaNacimiento = model.FechaNacimiento;
                _context.Entry(usuarioDb.Empleado).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                    return NotFound();
                else
                    throw;
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("Duplicate entry") == true && ex.InnerException?.Message.Contains("for key 'Email'") == true)
                    return Conflict(new { message = "El email ya está en uso por otro usuario." });
                throw;
            }

            return NoContent();
        }

        // PUT: api/Usuarios/5/password
        [HttpPut("{id}/password")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ChangeUserPassword(int id, [FromBody] ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            if (model.NewPassword != model.ConfirmNewPassword)
                return BadRequest(new { message = "Las nuevas contraseñas no coinciden." });

            usuario.PasswordHash = PasswordHelper.CreateHash(model.NewPassword);

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al cambiar la contraseña: {ex.Message}" });
            }
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var userIdFromToken = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserRole == "SuperAdmin" && id == userIdFromToken)
                return BadRequest(new { message = "Un SuperAdmin no puede eliminarse a sí mismo." });

            var usuario = await _context.Usuarios.Include(u => u.Empleado).FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)
                return NotFound();

            // RESTRICCIÓN: Si es Admin, solo puede eliminar empleados de su sucursal
            if (currentUserRole == "Admin")
            {
                var adminId = userIdFromToken;
                var adminSucursalId = await (from u in _context.Usuarios
                                             join e in _context.Empleados on u.EmpleadoId equals e.Id
                                             where u.Id == adminId
                                             select e.SucursalId).FirstOrDefaultAsync();
                if (usuario.Empleado == null || usuario.Empleado.SucursalId != adminSucursalId)
                    return Forbid("Solo puedes eliminar empleados de tu propia sucursal.");
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Usuarios/superadmin/usuarios-empleados
        // GET: api/Usuarios/superadmin/usuarios-empleados?skip=0&take=12
        [HttpGet("superadmin/usuarios-empleados")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<IEnumerable<UsuarioEmpleadoVM>>> GetUsuariosEmpleados([FromQuery] int skip = 0, [FromQuery] int take = 12)
        {
            var query = from u in _context.Usuarios
                        join e in _context.Empleados on u.EmpleadoId equals e.Id into empleadoJoin
                        from e in empleadoJoin.DefaultIfEmpty()
                        join s in _context.Sucursales on e.SucursalId equals s.Id into sucursalJoin
                        from s in sucursalJoin.DefaultIfEmpty()
                        join r in _context.Roles on u.RolId equals r.Id into rolJoin
                        from r in rolJoin.DefaultIfEmpty()
                        select new { u, e, s, r };

            var resultList = await query
                .OrderBy(x => x.u.Nombre)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            var result = resultList.Select(x => new UsuarioEmpleadoVM
            {
                UsuarioId = x.u.Id,
                Username = x.u.Username,
                UsuarioNombre = x.u.Nombre,
                UsuarioApellido = x.u.Apellido,
                EmpleadoId = x.e?.Id,
                EmpleadoNombre = x.e?.Nombre,
                EmpleadoApellido = x.e?.Apellido,
                FechaNacimiento = x.e?.FechaNacimiento,
                CorreoConcatenado = $"{(x.u.Nombre ?? x.e?.Nombre)?.ToLower()}.{(x.u.Apellido ?? x.e?.Apellido)?.ToLower()}@email.com",
                Telefono = x.e?.Telefono,
                SucursalNombre = x.s?.Nombre,
                NombreRol = x.r?.Nombre
            }).ToList();

            return Ok(result);
        }

        // GET: api/Usuarios/admin/usuarios-sucursal
        // GET: api/Usuarios/admin/usuarios-sucursal?skip=0&take=12
        [HttpGet("admin/usuarios-sucursal")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UsuarioEmpleadoVM>>> GetUsuariosDeMiSucursal([FromQuery] int skip = 0, [FromQuery] int take = 12)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var sucursalId = await (from u in _context.Usuarios
                                    join e in _context.Empleados on u.EmpleadoId equals e.Id
                                    where u.Id == userId
                                    select e.SucursalId)
                                    .FirstOrDefaultAsync();

            if (sucursalId == null)
                return BadRequest("No se encontró sucursal para este usuario.");

            var query = from u in _context.Usuarios
                        join e in _context.Empleados on u.EmpleadoId equals e.Id into empleadoJoin
                        from e in empleadoJoin.DefaultIfEmpty()
                        join s in _context.Sucursales on e.SucursalId equals s.Id into sucursalJoin
                        from s in sucursalJoin.DefaultIfEmpty()
                        join r in _context.Roles on u.RolId equals r.Id into rolJoin
                        from r in rolJoin.DefaultIfEmpty()
                        where e.SucursalId == sucursalId
                        select new { u, e, s, r };

            var resultList = await query
                .OrderBy(x => x.u.Nombre)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            var result = resultList.Select(x => new UsuarioEmpleadoVM
            {
                UsuarioId = x.u.Id,
                Username = x.u.Username,
                UsuarioNombre = x.u.Nombre,
                UsuarioApellido = x.u.Apellido,
                EmpleadoId = x.e?.Id,
                EmpleadoNombre = x.e?.Nombre,
                EmpleadoApellido = x.e?.Apellido,
                FechaNacimiento = x.e?.FechaNacimiento,
                CorreoConcatenado = $"{(x.u.Nombre ?? x.e?.Nombre)?.ToLower()}.{(x.u.Apellido ?? x.e?.Apellido)?.ToLower()}@email.com",
                Telefono = x.e?.Telefono,
                SucursalNombre = x.s?.Nombre,
                NombreRol = x.r?.Nombre
            }).ToList();

            return Ok(result);
        }

        // GET: api/Usuarios/contactos (público)
        [HttpGet("contactos")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<UsuarioContactoVM>>> GetContactosBasicos()
        {
            var usuarios = await _context.Usuarios
                .Where(u => u.Activo)
                .Select(u => new UsuarioContactoVM
                {
                    Id = u.Id,
                    Username = u.Username,
                    Nombre = u.Nombre
                }).ToListAsync();
            return Ok(usuarios);
        }

        // GET: api/Usuarios/buscar-contactos?search=xxx&sucursalId=yyy
        [HttpGet("buscar-contactos")]
        public async Task<ActionResult<IEnumerable<UsuarioContactoVM>>> BuscarContactos([FromQuery] string? search = null, [FromQuery] int? sucursalId = null)
        {
            var query = from u in _context.Usuarios
                        join e in _context.Empleados on u.EmpleadoId equals e.Id into empleadoJoin
                        from e in empleadoJoin.DefaultIfEmpty()
                        join s in _context.Sucursales on e.SucursalId equals s.Id into sucursalJoin
                        from s in sucursalJoin.DefaultIfEmpty()
                        where u.Activo
                        select new { u, e, s };

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => (x.u.Nombre + " " + x.u.Apellido + " " + x.u.Username + " " + (x.e != null ? x.e.Nombre : "") + " " + (x.e != null ? x.e.Apellido : ""))
                    .ToLower().Contains(search.ToLower()));
            }
            if (sucursalId.HasValue)
            {
                query = query.Where(x => x.e != null && x.e.SucursalId == sucursalId.Value);
            }

            var result = await query.Select(x => new UsuarioContactoVM
            {
                Id = x.u.Id,
                Username = x.u.Username,
                Nombre = x.u.Nombre ?? x.e.Nombre ?? string.Empty,
                Apellido = x.u.Apellido ?? x.e.Apellido ?? string.Empty,
                SucursalNombre = x.s != null ? x.s.Nombre : string.Empty
            }).ToListAsync();
            return Ok(result);
        }

        // PUT: api/Usuarios/cambiar-sucursal
        [HttpPut("cambiar-sucursal")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> CambiarSucursalEmpleado([FromBody] CambiarSucursalEmpleadoRequest request)
        {
            if (request.UsuarioId <= 0 || request.NuevaSucursalId <= 0)
                return BadRequest("Datos inválidos.");

            var usuario = await _context.Usuarios.Include(u => u.Empleado).FirstOrDefaultAsync(u => u.Id == request.UsuarioId);
            if (usuario == null || usuario.Empleado == null)
                return NotFound("Usuario o empleado no encontrado.");

            var sucursal = await _context.Sucursales.FindAsync(request.NuevaSucursalId);
            if (sucursal == null)
                return NotFound("Sucursal no encontrada.");

            usuario.Empleado.SucursalId = request.NuevaSucursalId;
            _context.Entry(usuario.Empleado).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Usuarios/me
        [HttpGet("me")]
        public async Task<ActionResult<object>> GetUsuarioActual()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Empleado)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (usuario == null)
                return NotFound();

            return Ok(new
            {
                Id = usuario.Id,
                Username = usuario.Username,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                RolId = usuario.RolId,
                RolNombre = usuario.Rol?.Nombre,
                SucursalId = usuario.Empleado?.SucursalId,
                EmpleadoId = usuario.EmpleadoId
            });
        }

        // GET: api/Usuarios/{id}/sucursal-id
        [HttpGet("{id}/sucursal-id")]
        [AllowAnonymous]
        public async Task<ActionResult<int?>> GetSucursalIdByUsuarioId(int id)
        {
            // Buscar el usuario y su EmpleadoId
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null || usuario.EmpleadoId == null)
                return NotFound("Usuario o empleado no encontrado.");

            // Buscar el empleado y su SucursalId
            var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.Id == usuario.EmpleadoId);
            if (empleado == null || empleado.SucursalId == null)
                return NotFound("El empleado no tiene sucursal asignada.");

            return Ok(empleado.SucursalId);
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}