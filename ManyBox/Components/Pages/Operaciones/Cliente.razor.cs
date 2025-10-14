using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace ManyBox.Components.Pages.Operaciones
{
    public partial class Cliente
    {
        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        private string busquedaCliente = "";
        private List<ClienteModel> clientesEncontrados = new();
        private string? errorMessage;
        private bool isLoadingBusqueda = false;
        private ClienteModel? clienteInfo = null;
        private int paginaActual = 1;
        private const int pageSize = 12;
        private bool mostrarBotonMostrarMas = false;

        protected override async Task OnInitializedAsync()
        {
            await BuscarClienteAsync(reset: true);
        }

        private async Task BuscarClienteAsync(bool reset = true)
        {
            errorMessage = null;
            isLoadingBusqueda = true;
            try
            {
                if (reset)
                {
                    paginaActual = 1;
                    clientesEncontrados.Clear();
                }
                int skip = (paginaActual - 1) * pageSize;
                var url = $"api/clientes?nombre={Uri.EscapeDataString(busquedaCliente)}&skip={skip}&take={pageSize}";
                var result = await Http.GetFromJsonAsync<List<ClienteModel>>(url);
                if (reset)
                {
                    clientesEncontrados = result ?? new List<ClienteModel>();
                }
                else
                {
                    clientesEncontrados.AddRange(result ?? new List<ClienteModel>());
                }
                mostrarBotonMostrarMas = (result?.Count ?? 0) == pageSize;
            }
            catch (Exception ex)
            {
                errorMessage = "Error buscando clientes: " + ex.Message;
            }
            finally
            {
                isLoadingBusqueda = false;
            }
        }

        private async Task MostrarMasClientes()
        {
            paginaActual++;
            await BuscarClienteAsync(reset: false);
        }

        private void MostrarInfoCliente(ClienteModel cliente)
        {
            clienteInfo = cliente;
        }

        private void CerrarInfoCliente()
        {
            clienteInfo = null;
        }

        private string GetInitials(string nombre, string apellido)
        {
            var n = string.IsNullOrWhiteSpace(nombre) ? "" : nombre.Trim()[0].ToString().ToUpper();
            var a = string.IsNullOrWhiteSpace(apellido) ? "" : apellido.Trim()[0].ToString().ToUpper();
            return n + a;
        }

        public class ClienteModel
        {
            public int Id { get; set; }
            public required string Nombre { get; set; }
            public required string Apellido { get; set; }
            public required string Correo { get; set; }
            public required string Telefono { get; set; }
        }
    }
}