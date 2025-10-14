using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ManyBox.Models.Api;
using ManyBox.Services;

namespace ManyBox.Components.Pages.Operaciones
{
    public partial class RegistrarSucursal : ComponentBase
    {
        [Inject] private SucursalApiService SucursalApiService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private Sucursal nuevaSucursal = new();
        private bool isLoading = false;
        private string? errorMsg;

        private async Task RegistrarSucursalAsync()
        {
            errorMsg = null;
            isLoading = true;
            try
            {
                var response = await SucursalApiService.RegistrarSucursalAsync(nuevaSucursal);
                if (response.IsSuccessStatusCode)
                {
                    Navigation.NavigateTo("/empleados");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    errorMsg = $"Error al registrar la sucursal: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                errorMsg = $"Error inesperado: {ex.Message}";
            }
            finally
            {
                isLoading = false;
            }
        }

        private void NavigateBack()
        {
            Navigation.NavigateTo("/empleados");
        }
    }
}
