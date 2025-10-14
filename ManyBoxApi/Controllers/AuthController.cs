using ManyBoxApi.Data;
using ManyBoxApi.Helpers;
using ManyBoxApi.Models;
using ManyBoxApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginVM login)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            // Forzar bool no-nullable por si el mapeo es bool?
            .FirstOrDefaultAsync(u => u.Username == login.Username && (u.Activo == true));

        if (usuario == null || !PasswordHelper.VerifyHash(login.Password, usuario.PasswordHash))
            return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

        var token = GenerateJwtToken(usuario);

        return Ok(new
        {
            token,
            user = new
            {
                usuario.Id,
                usuario.Username,
                usuario.Nombre,
                usuario.Apellido,
                Rol = usuario.Rol?.Nombre,
                usuario.Activo,
                EmpleadoId = usuario.EmpleadoId ?? usuario.Id // Agregado para la app móvil
            }
        });
    }

    private string GenerateJwtToken(Usuario usuario)
    {
        // Lee JWT desde variables de entorno, NO desde appsettings
        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
            ?? throw new InvalidOperationException("JWT_KEY not found in environment");
        var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "manybox-api";
        var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "manybox-app";
        var expireMinutes = Environment.GetEnvironmentVariable("JWT_EXPIRE_MINUTES") ?? "480"; // Puedes agregarlo al .env si quieres

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
        new Claim(ClaimTypes.Name, usuario.Username),
        new Claim(ClaimTypes.Role, usuario.Rol?.Nombre ?? "")
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(expireMinutes));

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}