using ManyBoxApi.Helpers;
using ManyBoxApi.Data;
using ManyBoxApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ManyBoxApi.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Valida el usuario por username y password contra la base de datos.
        /// </summary>
        public async Task<AuthResult> Login(string username, string password)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Username == username && u.Activo);

            // Verificar si el usuario existe y la contraseña es correcta
            if (usuario == null || !PasswordHelper.VerifyHash(password, usuario.PasswordHash))
            {
                return AuthResult.Fail("Credenciales inválidas.");
            }

            var token = GenerateToken(usuario);
            return AuthResult.Ok(token, usuario);
        }

        /// <summary>
        /// Registra un usuario y devuelve el token si es exitoso.
        /// </summary>
        public async Task<AuthResult> Registrar(Usuario usuario, string password)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Username == usuario.Username))
            {
                return AuthResult.Fail("El username ya está registrado.");
            }

            var rolExiste = await _context.Roles.AnyAsync(r => r.Id == usuario.RolId);
            if (!rolExiste)
            {
                return AuthResult.Fail($"El RolId {usuario.RolId} no es válido.");
            }

            usuario.PasswordHash = PasswordHelper.CreateHash(password);
            usuario.Activo = true;
            usuario.FechaCreacion = DateTime.UtcNow;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            await _context.Entry(usuario).Reference(u => u.Rol).LoadAsync();

            var token = GenerateToken(usuario);
            return AuthResult.Ok(token, usuario);
        }

        /// <summary>
        /// Genera el JWT token para un usuario autenticado.
        /// </summary>
        private string GenerateToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"]!;
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "ManyBoxApi";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "ManyBoxApiUsers";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                //new Claim(ClaimTypes.Email, usuario.Email ?? ""), // Agregar el email como claim
            };

            if (usuario.Rol != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, usuario.Rol.Nombre));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtIssuer,
                Audience = jwtAudience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Permite exponer la generación de token a otros servicios/controladores si lo necesitas.
        /// </summary>
        public string GenerateTokenForNewlyCreatedUser(Usuario usuario)
        {
            return GenerateToken(usuario);
        }
    }
}