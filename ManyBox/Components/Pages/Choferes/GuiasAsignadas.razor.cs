using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ManyBox.Services;
using ManyBox.Models;

namespace ManyBox.Components.Pages.Choferes
{
    public partial class GuiasAsignadas
    {
        [Inject] public GuiasChoferApiService GuiasService { get; set; } = default!;
        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IServiceProvider ServiceProvider { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private List<GuiaChoferDto> guiasAsignadas = new();
        private bool modalFirmaVisible = false;
        private GuiaChoferDto? guiaFirmaActual;
        private bool isDrawing = false;
        private double lastX, lastY;
        private bool isLoading = false;

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
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener guías: {ex.Message}");
            }
        }

        public async Task CambiarEstado(GuiaChoferDto guia, int nuevoEstado)
        {
            isLoading = true;
            string[] estados = { "En preparación", "En camino", "Último tramo", "Entregado" };
            string status = estados[nuevoEstado];
            var ok = await GuiasService.CambiarEstadoGuiaAsync(guia.EnvioId, status);
            if (ok)
            {
                await CargarEnvios(); // Refresca la lista tras el cambio
            }
            isLoading = false;
            StateHasChanged();
        }

        private void MostrarEntrega(GuiaChoferDto guia)
        {
            guiaFirmaActual = guia;
            modalFirmaVisible = true;
            _ = Task.Delay(100).ContinueWith(_ => ClearCanvas());
        }

        public async Task AceptarFirma()
        {
            if (guiaFirmaActual != null)
            {
                isLoading = true;
                var ok = await GuiasService.CambiarEstadoGuiaAsync(guiaFirmaActual.EnvioId, "Entregado");
                if (ok)
                {
                    await CargarEnvios(); // Refresca la lista tras la entrega
                    guiaFirmaActual = null;
                }
                isLoading = false;
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