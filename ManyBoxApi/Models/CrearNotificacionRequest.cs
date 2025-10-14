using System.Collections.Generic;
public class CrearNotificacionRequest
{
    // Datos de la notificación
    public string Tipo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public string? Datos { get; set; }
    public DateTime? Expiracion { get; set; }

    // Datos del destinatario
    public int UsuarioId { get; set; }
    public int SucursalId { get; set; }
    public int RolId { get; set; }
}
