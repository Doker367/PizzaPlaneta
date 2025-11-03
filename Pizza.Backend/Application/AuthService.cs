using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Pizza.Backend.Application.DTOs;
using Pizza.Backend.Domain;
using Pizza.Backend.Ports;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography; // Added for secure token generation

namespace Pizza.Backend.Application;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginUserDto loginUserDto)
    {
        var user = await _userRepository.GetUserByEmailAsync(loginUserDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginUserDto.Password, user.PasswordHash))
        {
            throw new Exception("Credenciales inválidas.");
        }

        var token = GenerateJwtToken(user);

        return new LoginResponseDto { Token = token };
    }

    public async Task RegisterAsync(RegisterUserDto registerUserDto)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(registerUserDto.Email);
        if (existingUser != null)
        {
            throw new Exception("El correo electrónico ya está en uso.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password);

        var newUser = new Usuario
        {
            Nombre = registerUserDto.Nombre,
            Email = registerUserDto.Email,
            Telefono = registerUserDto.Telefono,
            PasswordHash = passwordHash,
            FechaRegistro = DateTime.UtcNow
        };

        await _userRepository.AddUserAsync(newUser);
    }

    public async Task ForgotPassword(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null)
        {
            // Para evitar la enumeración de usuarios, no se debe indicar si el email existe o no.
            // Simplemente se retorna sin hacer nada o se envía un mensaje genérico de éxito.
            return; 
        }

        var tokenBytes = new byte[32]; // 256 bits
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(tokenBytes);
        }
        var resetToken = Convert.ToBase64String(tokenBytes); // This is the raw token to send via email

        user.PasswordResetToken = BCrypt.Net.BCrypt.HashPassword(resetToken); // Store hashed token in DB
        user.ResetTokenExpires = DateTime.UtcNow.AddMinutes(15); // Token valid for 15 minutes

        await _userRepository.UpdateUserAsync(user);

        // TODO: Implement email sending here.
        // The email should contain a link like:
        // https://your-frontend.com/reset-password?token={resetToken}&email={user.Email}
        Console.WriteLine($"Password reset token for {user.Email}: {resetToken}"); // For testing/debugging
    }

    public async Task ResetPassword(ResetPasswordDto dto)
    {
        var user = await _userRepository.GetUserByEmailAsync(dto.Email);

        if (user == null || user.PasswordResetToken == null || user.ResetTokenExpires == null || user.ResetTokenExpires < DateTime.UtcNow)
        {
            throw new Exception("Token de restablecimiento inválido o expirado.");
        }

        // Verify the provided token against the stored hashed token
        if (!BCrypt.Net.BCrypt.Verify(dto.Token, user.PasswordResetToken))
        {
            throw new Exception("Token de restablecimiento inválido o expirado.");
        }

        if (dto.Password != dto.ConfirmPassword)
        {
            throw new Exception("Las contraseñas no coinciden.");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        user.PasswordResetToken = null; // Invalidate token
        user.ResetTokenExpires = null; // Clear expiration

        await _userRepository.UpdateUserAsync(user);
    }

    private string GenerateJwtToken(Usuario user)
    {
        var jwtKey = _configuration["JWT_SECRET"] ?? throw new InvalidOperationException("JWT Secret key is not configured.");
        var jwtIssuer = _configuration["JWT_ISSUER"] ?? throw new InvalidOperationException("JWT Issuer is not configured.");
        var jwtAudience = _configuration["JWT_AUDIENCE"] ?? throw new InvalidOperationException("JWT Audience is not configured.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("nombre", user.Nombre),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
