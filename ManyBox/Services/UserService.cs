using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ManyBox.Models.Custom;
using ManyBox.Models.Api;
using Microsoft.Maui.Storage;
using System.Collections.Generic;
using System;
using System.Linq;

public class UserService
{
    private readonly HttpClient _http;
    public UserService(HttpClient http) => _http = http;

    public async Task<UsuarioVM?> GetUsuarioActualAsync()
    {
        // Asegura que el token JWT esté en la cabecera Authorization
        var token = Preferences.Default.Get("token", string.Empty);
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Remove("Authorization");
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        try
        {
            return await _http.GetFromJsonAsync<UsuarioVM>("api/usuarios/me");
        }
        catch (HttpRequestException)
        {
            // Si el token es inválido o expiró, retorna null para que la UI lo maneje
            return null;
        }
    }

    // Nuevo método para registrar empleados
    public async Task<HttpResponseMessage> RegistrarEmpleadoAsync(CreateUsuarioEmpleadoVM model)
    {
        var token = Preferences.Default.Get("token", string.Empty);
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Remove("Authorization");
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        return await _http.PostAsJsonAsync("api/usuarios", model);
    }

    // Cambiado: ahora usa el controller de empleados para paginación real
    public async Task<List<UsuarioEmpleadoVM>> GetUsuariosEmpleadosAsync(int skip = 0, int take = 12, string? search = null, string? sucursal = null, string? rol = null)
    {
        var token = Preferences.Default.Get("token", string.Empty);
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Remove("Authorization");
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        List<UsuarioEmpleadoVM>? result = null;
        if (!string.IsNullOrWhiteSpace(rol) && rol.Trim().ToLower() == "superadmin")
        {
            result = await _http.GetFromJsonAsync<List<UsuarioEmpleadoVM>>($"api/usuarios/superadmin/usuarios-empleados?skip={skip}&take={take}");
        }
        else if (!string.IsNullOrWhiteSpace(rol) && rol.Trim().ToLower() == "admin")
        {
            result = await _http.GetFromJsonAsync<List<UsuarioEmpleadoVM>>($"api/usuarios/admin/usuarios-sucursal?skip={skip}&take={take}");
        }
        else
        {
            // fallback: intenta superadmin
            result = await _http.GetFromJsonAsync<List<UsuarioEmpleadoVM>>($"api/usuarios/superadmin/usuarios-empleados?skip={skip}&take={take}");
        }
        return result ?? new List<UsuarioEmpleadoVM>();
    }

    public async Task<HttpResponseMessage> EliminarEmpleadoAsync(int usuarioId)
    {
        var token = Preferences.Default.Get("token", string.Empty);
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Remove("Authorization");
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        return await _http.DeleteAsync($"api/usuarios/{usuarioId}");
    }

    // Cambiar sucursal de empleado
    public async Task<HttpResponseMessage> CambiarSucursalEmpleadoAsync(int usuarioId, int nuevaSucursalId)
    {
        var token = Preferences.Default.Get("token", string.Empty);
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Remove("Authorization");
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        var request = new CambiarSucursalEmpleadoRequest
        {
            UsuarioId = usuarioId,
            NuevaSucursalId = nuevaSucursalId
        };
        return await _http.PutAsJsonAsync("api/usuarios/cambiar-sucursal", request);
    }

    public async Task<UsuarioEmpleadoDetalleVM?> GetEmpleadoDetalleAsync(int usuarioId)
    {
        var token = Preferences.Default.Get("token", string.Empty);
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Remove("Authorization");
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        try
        {
            return await _http.GetFromJsonAsync<UsuarioEmpleadoDetalleVM>($"api/usuarios/{usuarioId}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<HttpResponseMessage> ActualizarEmpleadoAsync(int usuarioId, UsuarioUpdateVM model)
    {
        var token = Preferences.Default.Get("token", string.Empty);
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Remove("Authorization");
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        return await _http.PutAsJsonAsync($"api/usuarios/{usuarioId}", model);
    }

    public async Task<HttpResponseMessage> CambiarPasswordEmpleadoAsync(int usuarioId, ChangePasswordVM model)
    {
        var token = Preferences.Default.Get("token", string.Empty);
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Remove("Authorization");
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        return await _http.PutAsJsonAsync($"api/usuarios/{usuarioId}/password", model);
    }
}