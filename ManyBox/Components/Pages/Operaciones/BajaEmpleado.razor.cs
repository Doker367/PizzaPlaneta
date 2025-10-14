using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ManyBox.Models.Api;
using ManyBox.Models.Custom;
using ManyBox.Services;

namespace ManyBox.Components.Pages.Operaciones
{
    public partial class BajaEmpleado : ComponentBase
    {
        [Inject] private UserService UserService { get; set; } = default!;
        [Inject] private SucursalApiService SucursalApiService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private List<UsuarioEmpleadoVM> empleados = new();
        private List<UsuarioEmpleadoVM> empleadosFiltrados = new();
        private List<Sucursal> sucursales = new();
        private string busquedaEmpleado = string.Empty;
        private string sucursalFiltro = string.Empty;
        private string? errorMsg;
        private bool modalVisible = false;
        private string modalTipo = "";
        private UsuarioEmpleadoVM? empleadoConfirmacionBaja;
        private UsuarioVM? usuarioActual;

        protected override async Task OnInitializedAsync()
        {
            await CargarSucursales();
            usuarioActual = await UserService.GetUsuarioActualAsync();
            await CargarEmpleados();
        }

        private async Task CargarSucursales()
        {
            sucursales = await SucursalApiService.GetSucursalesAsync();
        }

        private async Task CargarEmpleados()
        {
            empleados = await UserService.GetUsuariosEmpleadosAsync(0, 1000);
            FiltrarEmpleados();
        }

        private void FiltrarEmpleados()
        {
            empleadosFiltrados = empleados
                .Where(e => e.NombreRol != "SuperAdmin")
                .Where(e => (string.IsNullOrEmpty(busquedaEmpleado) ||
                            (e.UsuarioNombre + " " + e.UsuarioApellido + " " + e.Username).ToLower().Contains(busquedaEmpleado.ToLower())) &&
                            (string.IsNullOrEmpty(sucursalFiltro) || e.SucursalNombre == sucursalFiltro))
                .ToList();
        }

        private void OnBusquedaInput(ChangeEventArgs e)
        {
            busquedaEmpleado = e.Value?.ToString() ?? string.Empty;
            FiltrarEmpleados();
        }

        private void NavigateBack()
        {
            Navigation.NavigateTo("/empleados");
        }

        private void AbrirConfirmacionBaja(UsuarioEmpleadoVM emp)
        {
            empleadoConfirmacionBaja = emp;
            modalTipo = "confirmarBajaEmpleado";
            modalVisible = true;
            errorMsg = null;
        }

        private void CerrarModal()
        {
            modalVisible = false;
            modalTipo = "";
            errorMsg = null;
        }

        private async Task ConfirmarDarDeBajaEmpleado()
        {
            if (empleadoConfirmacionBaja != null)
            {
                errorMsg = null;
                // Prevenir que el usuario se dé de baja a sí mismo
                if (usuarioActual != null && empleadoConfirmacionBaja.UsuarioId == usuarioActual.Id)
                {
                    errorMsg = "No puedes darte de baja a ti mismo.";
                    return;
                }
                var response = await UserService.EliminarEmpleadoAsync(empleadoConfirmacionBaja.UsuarioId);
                if (response.IsSuccessStatusCode)
                {
                    await CargarEmpleados();
                    CerrarModal();
                }
                else
                {
                    errorMsg = "No se pudo dar de baja el empleado.";
                }
            }
        }
    }
}
