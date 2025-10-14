using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ManyBox.Models.Client; // DTO seguimiento local

namespace ManyBox.Components.Pages.Operaciones
{
    public partial class Seguimiento
    {
        [Inject] private HttpClient Http { get; set; } = default!;
        private string busquedaId = string.Empty;
        private PaqueteSeguimiento? paquete = null;
        private bool busquedaRealizada = false;

        private async Task BuscarPaquete()
        {
            paquete = null;
            busquedaRealizada = false;
            if (string.IsNullOrWhiteSpace(busquedaId))
                return;
            try
            {
                var result = await Http.GetFromJsonAsync<PaqueteSeguimiento>($"/api/paquetes/seguimiento/{busquedaId}");
                paquete = result;
            }
            catch
            {
                paquete = null;
            }
            busquedaRealizada = true;
        }

        private string GetStatusClass(string estado)
        {
            return estado.ToLower().Replace(" ", "-");
        }

        public class PaqueteSeguimiento
        {
            public string Id { get; set; } = string.Empty;
            public string Estado { get; set; } = string.Empty;
            public DateTime FechaActualizacion { get; set; }
            public string Origen { get; set; } = string.Empty;
            public string Destino { get; set; } = string.Empty;
            public List<MovimientoPaquete> Historial { get; set; } = new();
        }
        public class MovimientoPaquete
        {
            public DateTime Fecha { get; set; }
            public string Descripcion { get; set; } = string.Empty;
        }
    }
}
