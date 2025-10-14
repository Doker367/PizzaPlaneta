using Microsoft.AspNetCore.Components;
using ManyBox.Services;
using ManyBox.Models.Api;
using ManyBox.Models.Custom;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Json;

namespace ManyBox.Components.Pages.Operaciones
{
    public partial class CrearNotificacion : ComponentBase
    {
        [Inject] private HttpClient Http { get; set; } = default!;
        [Parameter] public EventCallback OnNotificacionCreada { get; set; }
        [Parameter] public EventCallback OnCancelar { get; set; }

        protected NotificacionesService.NuevaNotificacionDto nuevaNotificacion = new();
        protected string mensajeFeedback = string.Empty;

        protected List<UsuarioContactoVM> usuariosFiltrados = new();
        protected bool isBuscandoUsuarios = false;
        protected int myUserId = ManyBox.Utils.SessionState.IdUsuario;
        protected string filtroUsuario = string.Empty;
        protected List<Sucursal> sucursales = new();
        protected int? sucursalSeleccionadaId;

        protected override async Task OnInitializedAsync()
        {
            await CargarSucursalesAsync();
            // No buscar usuarios automáticamente
        }

        protected async Task CargarSucursalesAsync()
        {
            sucursales = await SucursalApiService.GetSucursalesAsync();
        }

        public async Task BuscarUsuariosAsync()
        {
            isBuscandoUsuarios = true;
            usuariosFiltrados = new();
            try
            {
                var todos = await ChatApiService.BuscarContactosAsync(filtroUsuario, sucursalSeleccionadaId);
                // Obtener sucursal de cada usuario desde la API (si no viene, hacer una consulta extra)
                foreach (var u in todos)
                {
                    if (u.SucursalId == null || u.SucursalId == 0)
                    {
                        // Consulta extra para obtener la sucursal del empleado
                        var detalle = await UserService.GetEmpleadoDetalleAsync(u.Id);
                        u.SucursalId = detalle?.SucursalId;
                    }
                }
                usuariosFiltrados = todos.Where(u => u.Id != myUserId).ToList();
            }
            catch { }
            isBuscandoUsuarios = false;
        }

        protected async Task CrearNotificacionAsync()
        {
            mensajeFeedback = string.Empty;
            var destinatarios = usuariosFiltrados.Where(u => u.Selected).ToList();
            if (destinatarios.Count == 0)
            {
                mensajeFeedback = "Selecciona al menos un usuario destinatario.";
                return;
            }
            try
            {
                foreach (var usuario in destinatarios)
                {
                    var dto = new NotificacionesService.NuevaNotificacionDto
                    {
                        Titulo = nuevaNotificacion.Titulo,
                        Mensaje = nuevaNotificacion.Mensaje,
                        Prioridad = nuevaNotificacion.Prioridad,
                        UsuarioId = usuario.Id,
                        // SucursalId eliminado, ya no se envía
                        // RolId: si lo necesitas, puedes mantenerlo
                    };
                    await NotificacionesService.CrearNotificacion(dto);
                }
                mensajeFeedback = "Notificación creada correctamente.";
                await OnNotificacionCreada.InvokeAsync();
            }
            catch (Exception ex)
            {
                mensajeFeedback = $"Error al crear notificación: {ex.Message}";
            }
        }
    }
}
