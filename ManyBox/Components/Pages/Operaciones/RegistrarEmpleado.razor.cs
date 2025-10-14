using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ManyBox.Models.Api;
using ManyBox.Services;
using ManyBox.Utils;

namespace ManyBox.Components.Pages.Operaciones
{
    public partial class RegistrarEmpleado : ComponentBase
    {
        [Inject] private UserService UserService { get; set; } = default!;
        [Inject] private SucursalApiService SucursalApiService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private CreateUsuarioEmpleadoVM nuevoEmpleado = new();
        private List<Sucursal> sucursales = new();
        private bool isLoading = false;
        private string? errorMsg;
        private List<RolVM> rolesDisponibles = new();
        private int? sucursalUnicaId = null;
        private string? sucursalUnicaNombre = null;

        protected override async Task OnInitializedAsync()
        {
            sucursales = await SucursalApiService.GetSucursalesAsync();
            CargarRolesDisponibles();
            await LimitarSucursalSiAdmin();
        }

        private async Task LimitarSucursalSiAdmin()
        {
            var usuario = await UserService.GetUsuarioActualAsync();
            if (usuario != null && (usuario.RolNombre?.Trim().ToLower() == "admin"))
            {
                sucursalUnicaId = usuario.SucursalId;
                sucursalUnicaNombre = sucursales.FirstOrDefault(s => s.Id == sucursalUnicaId)?.Nombre;
                // Limita la lista de sucursales solo a la del admin
                sucursales = sucursales.Where(s => s.Id == sucursalUnicaId).ToList();
                nuevoEmpleado.SucursalId = sucursalUnicaId ?? 0;
            }
        }

        private void CargarRolesDisponibles()
        {
            // Simulación de roles, normalmente vendrían de la base de datos
            var todosRoles = new List<RolVM>
            {
                new RolVM { Id = 1, Nombre = "SuperAdmin" },
                new RolVM { Id = 2, Nombre = "Admin" },
                new RolVM { Id = 3, Nombre = "empleado" },
                new RolVM { Id = 4, Nombre = "Chofer" }
            };
            var rolActual = (SessionState.Rol ?? "").Trim().ToLower();
            if (rolActual == "superadmin")
            {
                rolesDisponibles = todosRoles;
            }
            else if (rolActual == "admin")
            {
                rolesDisponibles = todosRoles.Where(r => r.Nombre == "empleado" || r.Nombre == "Chofer").ToList();
            }
            else
            {
                rolesDisponibles = new();
            }
        }

        private async Task RegistrarEmpleadoAsync()
        {
            errorMsg = null;
            isLoading = true;
            try
            {
                nuevoEmpleado.SucursalNombre = sucursales.FirstOrDefault(s => s.Id == nuevoEmpleado.SucursalId)?.Nombre;
                var response = await UserService.RegistrarEmpleadoAsync(nuevoEmpleado);
                if (response.IsSuccessStatusCode)
                {
                    Navigation.NavigateTo("/empleados");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    errorMsg = $"Error al registrar el empleado: {errorContent}";
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

        private class RolVM
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
        }
    }
}
