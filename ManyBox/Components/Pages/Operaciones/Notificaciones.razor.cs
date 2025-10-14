using Microsoft.AspNetCore.Components;
using ManyBox.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using ManyBox.Models.Client;
using ClientNotificacionDto = ManyBox.Models.Client.NotificacionFullDto;

namespace ManyBox.Components.Pages.Operaciones
{
    public partial class Notificaciones : ComponentBase, IAsyncDisposable
    {
        [Inject] public NotificacionesService NotificacionesService { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }

        protected List<ClientNotificacionDto> notificaciones = new();
        protected List<ClientNotificacionDto> notificacionesFiltradas = new();
        protected string filtroBusqueda = string.Empty;
        protected string filtroEstado = string.Empty;
        protected string filtroPrioridad = string.Empty;
        protected bool isLoading = false;
        protected string mensajeFeedback = string.Empty;
        protected bool mostrarModalNueva = false;
        protected NotificacionesService.NuevaNotificacionDto nuevaNotificacion = new();

        protected override async Task OnInitializedAsync()
        {
            await CargarNotificaciones();
        }

        protected async Task CargarNotificaciones()
        {
            isLoading = true;
            mensajeFeedback = string.Empty;
            try
            {
                notificaciones = await NotificacionesService.ObtenerNotificacionesUsuarioActual();
                AplicarFiltros();
            }
            catch (Exception ex)
            {
                mensajeFeedback = $"Error al cargar notificaciones: {ex.Message}";
            }
            isLoading = false;
        }

        protected void AplicarFiltros()
        {
            notificacionesFiltradas = notificaciones.Where(n =>
                (string.IsNullOrWhiteSpace(filtroPrioridad) || n.Prioridad == filtroPrioridad) &&
                (string.IsNullOrWhiteSpace(filtroEstado) || n.Estado == filtroEstado)
            ).ToList();
        }

        protected void MostrarModalNueva()
        {
            nuevaNotificacion = new NotificacionesService.NuevaNotificacionDto();
            mostrarModalNueva = true;
        }
        protected void CerrarModalNueva() => mostrarModalNueva = false;

        protected async Task CrearNotificacion()
        {
            if (string.IsNullOrWhiteSpace(nuevaNotificacion.Titulo) || string.IsNullOrWhiteSpace(nuevaNotificacion.Mensaje))
            {
                mensajeFeedback = "Completa todos los campos obligatorios.";
                return;
            }
            try
            {
                await NotificacionesService.CrearNotificacion(nuevaNotificacion);
                mensajeFeedback = "Notificación creada correctamente.";
                mostrarModalNueva = false;
                await CargarNotificaciones();
            }
            catch (Exception ex)
            {
                mensajeFeedback = $"Error al crear notificación: {ex.Message}";
            }
        }

        protected async Task MarcarComoLeida(ClientNotificacionDto noti)
        {
            try
            {
                await NotificacionesService.MarcarComoLeida(noti.Id);
                mensajeFeedback = "Notificación marcada como leída.";
                await CargarNotificaciones();
            }
            catch (Exception ex)
            {
                mensajeFeedback = $"Error al marcar como leída: {ex.Message}";
            }
        }

        protected async Task EliminarNotificacion(ClientNotificacionDto noti)
        {
            try
            {
                await NotificacionesService.EliminarNotificacion(noti.Id);
                mensajeFeedback = "Notificación eliminada.";
                await CargarNotificaciones();
            }
            catch (Exception ex)
            {
                mensajeFeedback = $"Error al eliminar: {ex.Message}";
            }
        }

        protected void OnFiltroChanged(ChangeEventArgs e)
        {
            AplicarFiltros();
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
