using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using ManyBox.Utils;
using System;
using ManyBox.Models.Client; // usar DTOs cliente

public class NotificacionesService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly NavigationManager _navigation;
    public NotificacionesService(IHttpClientFactory clientFactory, NavigationManager navigation)
    {
        _clientFactory = clientFactory;
        _navigation = navigation;
    }

    public async Task<List<NotificacionFullDto>> ObtenerNotificacionesUsuarioActual()
    {
        var client = _clientFactory.CreateClient("Api");
        int usuarioId = SessionState.IdUsuario;
        var response = await client.GetAsync($"api/notificaciones/usuario/{usuarioId}");
        if (!response.IsSuccessStatusCode)
            throw new Exception("No se pudieron obtener las notificaciones");
        var result = await response.Content.ReadFromJsonAsync<List<NotificacionFullDto>>();
        return result ?? new();
    }

    public async Task CrearNotificacion(NuevaNotificacionDto nueva)
    {
        var client = _clientFactory.CreateClient("Api");
        int usuarioId = nueva.UsuarioId ?? SessionState.IdUsuario;
        string rol = SessionState.Rol ?? "";
        int rolId = 0;
        int.TryParse(rol, out rolId);
        var request = new {
            Tipo = "info",
            Titulo = nueva.Titulo,
            Mensaje = nueva.Mensaje,
            Prioridad = nueva.Prioridad,
            UsuarioId = usuarioId,
            RolId = rolId
        };
        var response = await client.PostAsJsonAsync("api/notificaciones/crear-completa", request);
        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = await response.Content.ReadAsStringAsync();
            throw new Exception($"No se pudo crear la notificación: {errorMsg}");
        }
    }

    public async Task MarcarComoLeida(int notificacionId)
    {
        var client = _clientFactory.CreateClient("Api");
        int usuarioId = SessionState.IdUsuario;
        var response = await client.PutAsync($"api/notificaciones/entrega/{notificacionId}/leida?usuarioId={usuarioId}", null);
        if (!response.IsSuccessStatusCode)
            throw new Exception("No se pudo marcar como leída");
    }

    public async Task EliminarNotificacion(int notificacionId)
    {
        var client = _clientFactory.CreateClient("Api");
        var response = await client.DeleteAsync($"api/notificaciones/{notificacionId}");
        if (!response.IsSuccessStatusCode)
            throw new Exception("No se pudo eliminar la notificación");
    }

    public async Task<int> ObtenerNotificacionesNoLeidasCount()
    {
        int usuarioId = SessionState.IdUsuario;
        var client = _clientFactory.CreateClient("Api");
        var result = await client.GetFromJsonAsync<int>($"api/usuarios/{usuarioId}/notificaciones/no-leidas/count");
        return result;
    }

    public async Task MarcarNotificacionesComoLeidas()
    {
        int usuarioId = SessionState.IdUsuario;
        var client = _clientFactory.CreateClient("Api");
        var res = await client.PutAsync($"api/usuarios/{usuarioId}/notificaciones/marcar-todas-leidas", null);
        res.EnsureSuccessStatusCode();
    }

    public class NuevaNotificacionDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string Prioridad { get; set; } = "Media";
        public int? UsuarioId { get; set; }
        public int? SucursalId { get; set; }
        public int? RolId { get; set; }
    }
}

namespace ManyBox.Models.Client
{
    // DTO extendido para vista de notificaciones de usuario
    public class NotificacionFullDto : NotificacionDto
    {
        public int Id { get; set; }
        public string Estado { get; set; } = string.Empty;
        public bool Leido { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}