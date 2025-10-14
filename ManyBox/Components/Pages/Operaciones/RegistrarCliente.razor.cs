using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ManyBox.Components.Pages.Operaciones
{
    public partial class RegistrarCliente
    {
        [Inject] private HttpClient Http { get; set; } = default!;
        private ClienteModel nuevoCliente = new ClienteModel {
            Nombre = string.Empty,
            Apellido = string.Empty,
            Correo = string.Empty,
            Telefono = string.Empty
        };
        private bool isGuardandoCliente = false;
        private int? clienteRegistradoId = null;

        private async Task RegistrarClienteAsync()
        {
            isGuardandoCliente = true;
            try
            {
                var response = await Http.PostAsJsonAsync("api/clientes", nuevoCliente);
                if (response.IsSuccessStatusCode)
                {
                    var cliente = await response.Content.ReadFromJsonAsync<ClienteModel>();
                    clienteRegistradoId = cliente?.Id;
                    nuevoCliente = new ClienteModel {
                        Nombre = string.Empty,
                        Apellido = string.Empty,
                        Correo = string.Empty,
                        Telefono = string.Empty
                    };
                }
            }
            catch (Exception ex)
            {
                // Manejo de error
            }
            finally
            {
                isGuardandoCliente = false;
            }
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
