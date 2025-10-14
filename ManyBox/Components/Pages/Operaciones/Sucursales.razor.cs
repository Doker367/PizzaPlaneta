using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ManyBox.Models.Api;
using ManyBox.Services;
using System.Linq;

namespace ManyBox.Components.Pages.Operaciones
{
    public partial class Sucursales
    {
        [Inject] private SucursalApiService SucursalApiService { get; set; } = default!;

        private List<Sucursal> sucursales = new();
        private List<Sucursal> sucursalesFiltradas = new();
        private string busquedaSucursal = string.Empty;
        private bool mostrarRegistroSucursal = false;
        private Sucursal nuevaSucursal = new();
        private string? errorMsg;

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

        private void MostrarRegistroSucursal() => mostrarRegistroSucursal = true;

        private async Task RegistrarSucursalAsync()
        {
            errorMsg = null;
            var response = await SucursalApiService.RegistrarSucursalAsync(nuevaSucursal);
            if (response.IsSuccessStatusCode)
            {
                nuevaSucursal = new();
                mostrarRegistroSucursal = false;
                await CargarSucursales();
            }
            else
            {
                errorMsg = "No se pudo registrar la sucursal.";
            }
        }

        private async Task DarDeBajaSucursal(int sucursalId)
        {
            var response = await SucursalApiService.EliminarSucursalAsync(sucursalId);
            if (response.IsSuccessStatusCode)
            {
                await CargarSucursales();
            }
            else
            {
                errorMsg = "No se pudo dar de baja la sucursal.";
            }
        }
    }
}
