using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using ManyBox.Models.Locals;

namespace ManyBox.Services
{
    public class SyncService
    {
        private readonly LocalDatabaseService _localDb;
        private readonly HttpClient _httpClient;

        public SyncService(LocalDatabaseService localDb, HttpClient httpClient)
        {
            _localDb = localDb;
            _httpClient = httpClient;
        }

        // Sincroniza remitentes locales
        public async Task SyncRemitentesAsync()
        {
            var remitentes = await _localDb.GetAllRemitentesAsync();
            if (remitentes.Count > 0)
            {
                var response = await _httpClient.PostAsJsonAsync("https://tu-api.com/api/remitentes/sync", remitentes);
                response.EnsureSuccessStatusCode();
            }
        }

        // Sincroniza destinatarios locales
        public async Task SyncDestinatariosAsync()
        {
            var destinatarios = await _localDb.GetAllDestinatariosAsync();
            if (destinatarios.Count > 0)
            {
                var response = await _httpClient.PostAsJsonAsync("https://tu-api.com/api/destinatarios/sync", destinatarios);
                response.EnsureSuccessStatusCode();
            }
        }

        // Sincroniza paquetes locales
        public async Task SyncPaquetesAsync()
        {
            var paquetes = await _localDb.GetAllPaquetesAsync();
            if (paquetes.Count > 0)
            {
                var response = await _httpClient.PostAsJsonAsync("https://tu-api.com/api/paquetes/sync", paquetes);
                response.EnsureSuccessStatusCode();
            }
        }

        // Sincroniza envíos locales
        public async Task SyncEnviosAsync()
        {
            var envios = await _localDb.GetAllEnviosAsync();
            if (envios.Count > 0)
            {
                var response = await _httpClient.PostAsJsonAsync("https://tu-api.com/api/envios/sync", envios);
                response.EnsureSuccessStatusCode();
            }
        }

        // Sincroniza empleados locales
        public async Task SyncEmpleadosAsync()
        {
            var empleados = await _localDb.GetAllEmpleadosAsync();
            if (empleados.Count > 0)
            {
                var response = await _httpClient.PostAsJsonAsync("https://tu-api.com/api/empleados/sync", empleados);
                response.EnsureSuccessStatusCode();
            }
        }

        // Sincroniza contactos locales
        public async Task SyncContactosAsync()
        {
            var contactos = await _localDb.GetAllContactosAsync();
            if (contactos.Count > 0)
            {
                var response = await _httpClient.PostAsJsonAsync("https://tu-api.com/api/contactos/sync", contactos);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
    