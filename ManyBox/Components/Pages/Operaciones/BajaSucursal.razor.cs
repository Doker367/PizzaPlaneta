using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ManyBox.Models.Api;
using ManyBox.Services;

namespace ManyBox.Components.Pages.Operaciones
{
    public partial class BajaSucursal : ComponentBase
    {
        [Inject] private SucursalApiService SucursalApiService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private List<Sucursal> sucursales = new();
        private List<Sucursal> sucursalesFiltradas = new();
        private string busquedaSucursal = string.Empty;
        private string? errorMsg;
        private bool modalVisible = false;
        private Sucursal? sucursalConfirmacionBaja;

        protected override async Task OnInitializedAsync()
        {
            await CargarSucursales();
        }

        private async Task CargarSucursales()
        {
            sucursales = await SucursalApiService.GetSucursalesAsync();
            FiltrarSucursales();
        }

        private void FiltrarSucursales()
        {
            sucursalesFiltradas = sucursales
                .Where(s => string.IsNullOrEmpty(busquedaSucursal) || s.Nombre.ToLower().Contains(busquedaSucursal.ToLower()))
                .ToList();
        }

        private void OnBusquedaInput(ChangeEventArgs e)
        {
            busquedaSucursal = e.Value?.ToString() ?? string.Empty;
            FiltrarSucursales();
        }

        private void NavigateBack()
        {
            Navigation.NavigateTo("/empleados");
        }

        private void AbrirConfirmacionBaja(Sucursal suc)
        {
            sucursalConfirmacionBaja = suc;
            modalVisible = true;
            errorMsg = null;
        }

        private void CerrarModal()
        {
            modalVisible = false;
            sucursalConfirmacionBaja = null;
            errorMsg = null;
        }

        private async Task ConfirmarDarDeBajaSucursal()
        {
            if (sucursalConfirmacionBaja != null)
            {
                errorMsg = null;
                var response = await SucursalApiService.EliminarSucursalAsync(sucursalConfirmacionBaja.Id);
                if (response.IsSuccessStatusCode)
                {
                    await CargarSucursales();
                    CerrarModal();
                }
                else
                {
                    errorMsg = "No se pudo dar de baja la sucursal.";
                }
            }
        }
    }
}
