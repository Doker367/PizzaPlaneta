using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ManyBox.Utils;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace ManyBox.Components.Pages.Auth
{
    public partial class Login
    {
        [Inject] NavigationManager Navigation { get; set; } = default!;
        [Inject] IHttpClientFactory ClientFactory { get; set; } = default!;

        private LoginModel loginModel = new();
        private bool loginExitoso = false;
        private string errorMessage = string.Empty;
        private bool isLoading = false;

        private async Task OnLogin()
        {
            errorMessage = string.Empty;
            isLoading = true;

            var inputUsuario = loginModel.Usuario?.Trim() ?? string.Empty;
            var inputPassword = loginModel.Password?.Trim() ?? string.Empty;

            try
            {
                var client = ClientFactory.CreateClient("Api");
                var apiUrl = "api/Auth/login";
                var request = new
                {
                    Username = inputUsuario,
                    Password = inputPassword
                };

                var response = await client.PostAsJsonAsync(apiUrl, request);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResult>();
                    Console.WriteLine("Login result: " + (result != null ? "OK" : "NULL"));
                    Console.WriteLine("Token: " + result?.Token);

                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        // Guarda en SessionState y en Preferences para persistencia
                        SessionState.IsLoggedIn = true;
                        SessionState.Rol = result.User.Rol;
                        SessionState.Usuario = result.User.Username;
                        SessionState.IdUsuario = result.User.Id;
                        SessionState.Token = result.Token;

                        Preferences.Default.Set("token", result.Token);
                        Preferences.Default.Set("usuario", result.User.Username);
                        Preferences.Default.Set("rol", result.User.Rol);
                        Preferences.Default.Set("idUsuario", result.User.Id);

                        // Obtener EmpleadoId del usuario logueado
                        try
                        {
                            var clientMe = ClientFactory.CreateClient("Api");
                            var me = await clientMe.GetFromJsonAsync<UsuarioMeResult>("api/usuarios/me");
                            if (me != null && me.EmpleadoId.HasValue)
                            {
                                SessionState.EmpleadoId = me.EmpleadoId.Value;
                                Preferences.Default.Set("empleadoId", me.EmpleadoId.Value);
                            }
                        }
                        catch { /* Ignorar error, pero dejar EmpleadoId en 0 si falla */ }

                        loginExitoso = true;
                        await Task.Delay(100);
                        switch ((result.User.Rol?.ToLower() ?? ""))
                        {
                            case "superadmin":
                                Navigation.NavigateTo("/superadmin-dashboard", forceLoad: true);
                                break;
                            case "admin":
                                Navigation.NavigateTo("/admin-dashboard", forceLoad: true);
                                break;
                            case "empleado":
                                Navigation.NavigateTo("/employee-dashboard", forceLoad: true);
                                break;
                            case "chofer":
                                Navigation.NavigateTo("/chofer-dashboard", forceLoad: true);
                                break;
                            default:
                                Navigation.NavigateTo("/", forceLoad: true);
                                break;
                        }
                        isLoading = false;
                        return;
                    }
                }
                errorMessage = "Usuario o contraseña incorrectos";
                loginExitoso = false;
            }
            catch
            {
                errorMessage = "No se pudo conectar con el servidor. Intenta nuevamente.";
                loginExitoso = false;
            }

            isLoading = false;
        }

        public class LoginModel
        {
            public string Usuario { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        private class LoginResult
        {
            public string Token { get; set; }
            public UserResult User { get; set; }
        }
        private class UserResult
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Rol { get; set; }
            public bool Activo { get; set; }
        }
        private class UsuarioMeResult
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public int? EmpleadoId { get; set; }
            public int? SucursalId { get; set; }
            public int RolId { get; set; }
            public string RolNombre { get; set; }
        }
    }
}