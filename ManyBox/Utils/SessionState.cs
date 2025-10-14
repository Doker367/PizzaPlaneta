using System;

namespace ManyBox.Utils
{
    public static class SessionState
    {
        public static bool IsLoggedIn { get; set; } = false;
        public static string Usuario { get; set; } = string.Empty;
        public static string Rol { get; set; } = string.Empty;
        public static int IdUsuario { get; set; }
        public static int IdSucursal { get; set; }
        public static string Token { get; set; } = string.Empty;
        public static int EmpleadoId { get; set; } // Debe ser seteado al iniciar sesión
    }
}