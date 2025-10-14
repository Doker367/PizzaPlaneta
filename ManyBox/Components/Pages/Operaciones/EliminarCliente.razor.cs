using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace ManyBox.Components.Pages.Operaciones
{
    public partial class EliminarCliente
    {
        [Parameter] public int id { get; set; }
        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        private ClienteModel? cliente;
        private bool isEliminando = false;
        private string? errorEliminar;

        protected override async Task OnInitializedAsync()
        {
            cliente = await Http.GetFromJsonAsync<ClienteModel>($"api/clientes/{id}");
        }

        private async Task EliminarClienteAsync()
        {
            isEliminando = true;
            errorEliminar = null;
            try
            {
                var response = await Http.DeleteAsync($"api/clientes/{id}");
                if (response.IsSuccessStatusCode)
                {
                    NavigationManager.NavigateTo("/cliente");
                }
                else
                {
                    errorEliminar = $"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
                }
            }
            catch (Exception ex)
            {
                errorEliminar = ex.Message;
            }
            finally
            {
                isEliminando = false;
            }
        }

        private void Volver()
        {
            NavigationManager.NavigateTo("/cliente");
        }

        public class ClienteModel
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Apellido { get; set; } = string.Empty;
            public string Correo { get; set; } = string.Empty;
            public string Telefono { get; set; } = string.Empty;
        }
    }
}
