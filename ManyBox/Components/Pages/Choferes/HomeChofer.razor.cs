using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManyBox.Services;
using ManyBox.Models;

namespace ManyBox.Components.Pages.Choferes
{
    public partial class HomeChofer
    {
        [Inject] NavigationManager Navigation { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] GuiasChoferApiService GuiasService { get; set; } = default!;

        // Datos resumen
        private int paquetesPendientes = 0;
        private int paquetesConfirmados = 0;
        private int paquetesIncidencia = 0;
        private List<GuiaChoferDto> guiasAsignadas = new();

        // Firma modal
        private bool modalFirmaVisible = false;
        private GuiaChoferDto? guiaFirmaActual;

        // Canvas firma
        private bool isDrawing = false;
        private double lastX, lastY;

        protected override async Task OnInitializedAsync()
        {
            await CargarEnvios();
        }

        private async Task<int> ObtenerEmpleadoIdActualAsync()
        {
            // Usa el empleadoId real del usuario logueado
            return ManyBox.Utils.SessionState.EmpleadoId;
        }

        private async Task CargarEnvios()
        {
            try
            {
                int empleadoId = await ObtenerEmpleadoIdActualAsync();
                var guias = await GuiasService.ObtenerGuiasPorChoferAsync(empleadoId);
                guiasAsignadas = guias.Where(g => g.EstadoActual != 3).ToList();
                paquetesPendientes = guiasAsignadas.Count;
                paquetesConfirmados = guias.Count(g => g.EstadoActual == 3); // Entregados
                paquetesIncidencia = guias.Count(g => EsIncidencia(g));
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener guías: {ex.Message}");
            }
        }

        private bool EsIncidencia(GuiaChoferDto guia)
        {
            // Ajusta los estados de incidencia según tu lógica
            var estadosIncidencia = new[] { "Devuelto", "Extraviado", "Incidencia" };
            return guia.Estados.Any(e => estadosIncidencia.Contains(e.Titulo, StringComparer.OrdinalIgnoreCase));
        }

        private async Task CambiarEstado(GuiaChoferDto guia, int nuevoEstado)
        {
            string[] estados = { "En preparación", "En camino", "Último tramo", "Entregado" };
            string status = estados[nuevoEstado];
            var ok = await GuiasService.CambiarEstadoGuiaAsync(guia.EnvioId, status);
            if (ok)
            {
                guia.EstadoActual = nuevoEstado;
                if (guia.Estados.Count > nuevoEstado)
                    guia.Estados[nuevoEstado].FechaHora = DateTime.Now;
                if (nuevoEstado == 3)
                {
                    guiasAsignadas.Remove(guia);
                    paquetesPendientes = guiasAsignadas.Count;
                }
            }
            StateHasChanged();
        }

        private void MostrarEntrega(GuiaChoferDto guia)
        {
            guiaFirmaActual = guia;
            modalFirmaVisible = true;
            _ = Task.Delay(100).ContinueWith(_ => ClearCanvas());
        }

        private async Task AceptarFirma()
        {
            if (guiaFirmaActual != null)
            {
                var ok = await GuiasService.CambiarEstadoGuiaAsync(guiaFirmaActual.EnvioId, "Entregado");
                if (ok)
                {
                    guiaFirmaActual.EstadoActual = 3;
                    if (guiaFirmaActual.Estados.Count > 3)
                        guiaFirmaActual.Estados[3].FechaHora = DateTime.Now;
                    guiasAsignadas.Remove(guiaFirmaActual);
                    paquetesPendientes = guiasAsignadas.Count;
                    guiaFirmaActual = null;
                }
            }
            modalFirmaVisible = false;
            StateHasChanged();
        }

        private void CerrarModalFirma()
        {
            guiaFirmaActual = null;
            modalFirmaVisible = false;
        }

        private void ToggleExpand(GuiaChoferDto guia)
        {
            guia.Expandido = !guia.Expandido;
            StateHasChanged();
        }

        private void EscanearQR()
        {
            Navigation.NavigateTo("/qrscanner");
        }

        // Métodos firma canvas
        private async void ClearCanvas()
        {
            await JS.InvokeVoidAsync("firmaCanvas.clear");
        }
        private async Task StartDraw(Microsoft.AspNetCore.Components.Web.PointerEventArgs e)
        {
            isDrawing = true;
            lastX = e.ClientX;
            lastY = e.ClientY;
        }
        private async Task EndDraw(Microsoft.AspNetCore.Components.Web.PointerEventArgs e)
        {
            isDrawing = false;
        }
        private async Task Draw(Microsoft.AspNetCore.Components.Web.PointerEventArgs e)
        {
            if (!isDrawing) return;
            await JS.InvokeVoidAsync("firmaCanvas.draw", lastX, lastY, e.ClientX, e.ClientY);
            lastX = e.ClientX;
            lastY = e.ClientY;
        }
    }
}