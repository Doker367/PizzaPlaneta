using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ManyBox.Models.Api;
using ManyBox.Services;
using System.Linq;
using ManyBox.Utils;

namespace ManyBox.Components.Pages.Operaciones
{
    public partial class Empleados
    {
        [Inject] private UserService UserService { get; set; } = default!;
        [Inject] private SucursalApiService SucursalApiService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private CreateUsuarioEmpleadoVM nuevoEmpleado = new();
        private Sucursal nuevaSucursal = new();
        private int? sucursalBajaId;
        private int? empleadoBajaId;
        private string? errorMsg;
        private bool isLoading = false;
        private List<string> erroresValidacion = new();

        private List<UsuarioEmpleadoVM> empleados = new();
        private List<UsuarioEmpleadoVM> empleadosFiltrados = new();
        private List<Sucursal> sucursales = new();
        private string busquedaEmpleado = string.Empty;
        private string sucursalFiltro = string.Empty;

        private string usuarioActual = string.Empty;
        private string rolActual = string.Empty;

        private bool modalVisible = false;
        private string modalTipo = "";

        // Para cambiar sucursal
        private UsuarioEmpleadoVM? empleadoCambioSucursal;
        private int? nuevaSucursalEmpleadoId;

        // Para confirmacin de baja
        private UsuarioEmpleadoVM? empleadoConfirmacionBaja;

        // Para info empleado
        public UsuarioEmpleadoDetalleVM? empleadoDetalleVM;

        // Para actualizar info
        private UsuarioEmpleadoVM? empleadoActualizar;
        private UsuarioUpdateVM empleadoUpdateModel = new();
        private bool isUpdatingEmpleado = false;

        // Para cambiar password
        private UsuarioEmpleadoVM? empleadoCambiarPassword;
        private ChangePasswordVM passwordModel = new();
        private bool isChangingPassword = false;

        private int paginaActual = 1;
        private const int pageSize = 12;
        private bool mostrarBotonMostrarMas = false;

        protected override async Task OnInitializedAsync()
        {
            await CargarSucursales();
            await ObtenerUsuarioActual();
            await CargarEmpleados(reset: true);
        }

        private async Task ObtenerUsuarioActual()
        {
            var usuario = await UserService.GetUsuarioActualAsync();
            usuarioActual = usuario?.Username ?? string.Empty;
            rolActual = usuario?.RolNombre ?? string.Empty;
        }

        private async Task CargarSucursales()
        {
            sucursales = await SucursalApiService.GetSucursalesAsync();
        }

        private async Task CargarEmpleados(bool reset = true)
        {
            try
            {
                if (reset)
                {
                    paginaActual = 1;
                    empleados.Clear();
                }
                int skip = (paginaActual - 1) * pageSize;
                var nuevos = await UserService.GetUsuariosEmpleadosAsync(skip, pageSize, null, null, rolActual);
                if (reset)
                    empleados = nuevos.ToList();
                else
                    empleados.AddRange(nuevos);
                FiltrarEmpleados();
                mostrarBotonMostrarMas = (nuevos.Count() == pageSize);
            }
            catch (HttpRequestException ex)
            {
                errorMsg = "No tienes permisos para ver empleados o hubo un error de conexión.";
                empleados = new();
                empleadosFiltrados = new();
            }
        }

        private void FiltrarEmpleados()
        {
            if ((rolActual ?? "").Trim().ToLower() == "admin")
            {
                empleadosFiltrados = empleados
                    .Where(e => e.NombreRol == "empleado" || e.NombreRol == "Chofer")
                    .Where(e => (string.IsNullOrEmpty(busquedaEmpleado) ||
                                (e.UsuarioNombre + " " + e.UsuarioApellido + " " + e.Username).ToLower().Contains(busquedaEmpleado.ToLower())) &&
                                (string.IsNullOrEmpty(sucursalFiltro) || e.SucursalNombre == sucursalFiltro))
                    .ToList();
            }
            else
            {
                empleadosFiltrados = empleados
                    .Where(e => e.NombreRol != "SuperAdmin")
                    .Where(e => (string.IsNullOrEmpty(busquedaEmpleado) ||
                                (e.UsuarioNombre + " " + e.UsuarioApellido + " " + e.Username).ToLower().Contains(busquedaEmpleado.ToLower())) &&
                                (string.IsNullOrEmpty(sucursalFiltro) || e.SucursalNombre == sucursalFiltro))
                    .ToList();
            }
        }

        private async Task RegistrarEmpleadoAsync()
        {
            errorMsg = null;
            erroresValidacion.Clear();
            isLoading = true;
            try
            {
                nuevoEmpleado.SucursalNombre = sucursales.FirstOrDefault(s => s.Id == nuevoEmpleado.SucursalId)?.Nombre;
                var response = await UserService.RegistrarEmpleadoAsync(nuevoEmpleado);
                if (response.IsSuccessStatusCode)
                {
                    nuevoEmpleado = new();
                    await CargarEmpleados();
                    CerrarModal();
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

        private async Task RegistrarSucursalAsync()
        {
            errorMsg = null;
            try
            {
                var response = await SucursalApiService.RegistrarSucursalAsync(nuevaSucursal);
                if (response.IsSuccessStatusCode)
                {
                    nuevaSucursal = new();
                    await CargarSucursales();
                    CerrarModal();
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
        }

        private async Task DarDeBajaSucursalAsync()
        {
            errorMsg = null;
            if (sucursalBajaId == null)
            {
                errorMsg = "Selecciona una sucursal.";
                return;
            }
            var response = await SucursalApiService.EliminarSucursalAsync(sucursalBajaId.Value);
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

        private async Task DarDeBajaEmpleadoModalAsync()
        {
            errorMsg = null;
            if (empleadoBajaId == null)
            {
                errorMsg = "Selecciona un empleado.";
                return;
            }
            var empleado = empleados.FirstOrDefault(e => e.UsuarioId == empleadoBajaId);
            if (empleado != null && empleado.NombreRol == "SuperAdmin" && empleado.Username == usuarioActual)
            {
                errorMsg = "No puedes darte de baja como SuperAdmin.";
                return;
            }
            var response = await UserService.EliminarEmpleadoAsync(empleadoBajaId.Value);
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

        private void MostrarModal(string tipo)
        {
            modalTipo = tipo;
            modalVisible = true;
            errorMsg = null;
            if (tipo == "registroEmpleado") nuevoEmpleado = new();
            if (tipo == "registroSucursal") nuevaSucursal = new();
            if (tipo == "bajaSucursal") sucursalBajaId = null;
            if (tipo == "bajaEmpleado") empleadoBajaId = null;
        }
        private void CerrarModal()
        {
            modalVisible = false;
            modalTipo = "";
            errorMsg = null;
        }

        private async Task DarDeBajaEmpleado(int usuarioId)
        {
            var empleado = empleados.FirstOrDefault(e => e.UsuarioId == usuarioId);
            if (empleado != null && empleado.NombreRol == "SuperAdmin" && empleado.Username == usuarioActual)
            {
                errorMsg = "No puedes darte de baja como SuperAdmin.";
                return;
            }
            var response = await UserService.EliminarEmpleadoAsync(usuarioId);
            if (response.IsSuccessStatusCode)
            {
                await CargarEmpleados();
            }
            else
            {
                errorMsg = "No se pudo dar de baja el empleado.";
            }
        }

        // Cambiar sucursal empleado
        private async Task ConfirmarCambioSucursalEmpleado()
        {
            if (empleadoCambioSucursal == null || nuevaSucursalEmpleadoId == null)
            {
                errorMsg = "Selecciona una sucursal.";
                return;
            }
            isLoading = true;
            errorMsg = null;
            try
            {
                var response = await UserService.CambiarSucursalEmpleadoAsync(empleadoCambioSucursal.UsuarioId, nuevaSucursalEmpleadoId.Value);
                if (response.IsSuccessStatusCode)
                {
                    await CargarEmpleados();
                    CerrarModal();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    errorMsg = $"Error al cambiar la sucursal: {errorContent}";
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

        // Confirmacin de baja desde tarjeta
        private void AbrirConfirmacionBaja(UsuarioEmpleadoVM emp)
        {
            empleadoConfirmacionBaja = emp;
            modalTipo = "confirmarBajaEmpleado";
            modalVisible = true;
            errorMsg = null;
        }
        private async Task ConfirmarDarDeBajaEmpleado()
        {
            if (empleadoConfirmacionBaja != null)
            {
                await DarDeBajaEmpleado(empleadoConfirmacionBaja.UsuarioId);
                CerrarModal();
            }
        }
        // Modal para cambiar sucursal
        private void AbrirModalCambioSucursal(UsuarioEmpleadoVM emp)
        {
            empleadoCambioSucursal = emp;
            nuevaSucursalEmpleadoId = null;
            modalTipo = "cambiarSucursalEmpleado";
            modalVisible = true;
            errorMsg = null;
        }

        // Para info empleado
        private async Task MostrarInfoEmpleado(UsuarioEmpleadoVM emp)
        {
            isLoading = true;
            errorMsg = null;
            empleadoDetalleVM = null;
            try
            {
                var detalle = await UserService.GetEmpleadoDetalleAsync(emp.UsuarioId);
                if (detalle != null)
                {
                    empleadoDetalleVM = detalle;
                    modalTipo = "infoEmpleado";
                    modalVisible = true;
                }
                else
                {
                    errorMsg = "No se pudo obtener la información del empleado.";
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

        private void OnBusquedaInput(ChangeEventArgs e)
        {
            busquedaEmpleado = e.Value?.ToString() ?? string.Empty;
            FiltrarEmpleados();
        }

        private void AbrirModalActualizarEmpleado(UsuarioEmpleadoVM emp)
        {
            empleadoActualizar = emp;
            empleadoUpdateModel = new UsuarioUpdateVM
            {
                Id = emp.UsuarioId,
                Nombre = emp.UsuarioNombre,
                Apellido = emp.UsuarioApellido,
                Email = emp.CorreoConcatenado ?? string.Empty,
                Telefono = emp.Telefono,
                FechaNacimiento = emp.FechaNacimiento,
                RolId = null,
                Activo = null
            };
            modalTipo = "actualizarEmpleado";
            modalVisible = true;
            errorMsg = null;
        }

        private async Task ConfirmarActualizarEmpleado()
        {
            isUpdatingEmpleado = true;
            errorMsg = null;
            try
            {
                var response = await UserService.ActualizarEmpleadoAsync(empleadoUpdateModel.Id, empleadoUpdateModel);
                if (response.IsSuccessStatusCode)
                {
                    // Refresca la lista y el detalle si el modal de info sigue abierto
                    await CargarEmpleados();
                    if (empleadoDetalleVM != null && empleadoDetalleVM.UsuarioId == empleadoUpdateModel.Id)
                    {
                        var detalle = await UserService.GetEmpleadoDetalleAsync(empleadoUpdateModel.Id);
                        if (detalle != null)
                        {
                            empleadoDetalleVM = detalle;
                        }
                    }
                    CerrarModal();
                    // Vuelve a abrir el modal de info con los datos actualizados
                    if (empleadoDetalleVM != null && empleadoDetalleVM.UsuarioId == empleadoUpdateModel.Id)
                    {
                        await MostrarInfoEmpleado(new UsuarioEmpleadoVM {
                            UsuarioId = empleadoDetalleVM.UsuarioId,
                            UsuarioNombre = empleadoDetalleVM.UsuarioNombre,
                            UsuarioApellido = empleadoDetalleVM.UsuarioApellido,
                            Username = empleadoDetalleVM.Username,
                            CorreoConcatenado = empleadoDetalleVM.Correo,
                            NombreRol = empleadoDetalleVM.NombreRol
                        });
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    errorMsg = $"Error al actualizar: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                errorMsg = $"Error inesperado: {ex.Message}";
            }
            finally
            {
                isUpdatingEmpleado = false;
            }
        }

        private void AbrirModalCambiarPassword(UsuarioEmpleadoVM emp)
        {
            empleadoCambiarPassword = emp;
            passwordModel = new ChangePasswordVM();
            modalTipo = "cambiarPasswordEmpleado";
            modalVisible = true;
            errorMsg = null;
        }

        private async Task ConfirmarCambiarPassword()
        {
            isChangingPassword = true;
            errorMsg = null;
            try
            {
                var response = await UserService.CambiarPasswordEmpleadoAsync(empleadoCambiarPassword.UsuarioId, passwordModel);
                if (response.IsSuccessStatusCode)
                {
                    // Refresca la lista completa de empleados
                    await CargarEmpleados();
                    // Refresca el detalle si el modal de info sigue abierto
                    if (empleadoDetalleVM != null && empleadoDetalleVM.UsuarioId == empleadoCambiarPassword.UsuarioId)
                    {
                        var detalle = await UserService.GetEmpleadoDetalleAsync(empleadoCambiarPassword.UsuarioId);
                        if (detalle != null)
                        {
                            empleadoDetalleVM = detalle;
                        }
                    }
                    CerrarModal();
                    // Vuelve a abrir el modal de info con los datos actualizados
                    if (empleadoDetalleVM != null && empleadoDetalleVM.UsuarioId == empleadoCambiarPassword.UsuarioId)
                    {
                        await MostrarInfoEmpleado(new UsuarioEmpleadoVM {
                            UsuarioId = empleadoDetalleVM.UsuarioId,
                            UsuarioNombre = empleadoDetalleVM.UsuarioNombre,
                            UsuarioApellido = empleadoDetalleVM.UsuarioApellido,
                            Username = empleadoDetalleVM.Username,
                            CorreoConcatenado = empleadoDetalleVM.Correo,
                            NombreRol = empleadoDetalleVM.NombreRol
                        });
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    errorMsg = $"Error al cambiar contraseña: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                errorMsg = $"Error inesperado: {ex.Message}";
            }
            finally
            {
                isChangingPassword = false;
            }
        }

        private async Task MostrarMasEmpleados()
        {
            paginaActual++;
            await CargarEmpleados(reset: false);
        }
    }
}
