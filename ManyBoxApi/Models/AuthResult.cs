using ManyBoxApi.Models; // Necesario para la propiedad Usuario

public class AuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; } // Puede ser nulo si Success es false
    public string? ErrorMessage { get; set; } // Puede ser nulo si Success es true
    public Usuario? User { get; set; } // Puede ser nulo

    public static AuthResult Ok(string token, Usuario? user = null) // Ajustar aquí también
    {
        return new AuthResult { Success = true, Token = token, User = user, ErrorMessage = null };
    }

    public static AuthResult Fail(string errorMessage)
    {
        return new AuthResult { Success = false, ErrorMessage = errorMessage, Token = null, User = null };
    }
}