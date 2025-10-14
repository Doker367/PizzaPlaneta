using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManyBox.Services;

namespace ManyBox.ViewModels
{
    public class SyncViewModel
    {
        private readonly SyncService _syncService;

        public SyncViewModel(SyncService syncService)
        {
            _syncService = syncService;
        }

        public async Task SincronizarTodoAsync()
        {
            await _syncService.SyncRemitentesAsync();
            await _syncService.SyncDestinatariosAsync();
            await _syncService.SyncPaquetesAsync();
            await _syncService.SyncEnviosAsync();
            await _syncService.SyncEmpleadosAsync();
            await _syncService.SyncContactosAsync();
        }

        public async Task SincronizarRemitentesAsync() => await _syncService.SyncRemitentesAsync();
        public async Task SincronizarDestinatariosAsync() => await _syncService.SyncDestinatariosAsync();
        public async Task SincronizarPaquetesAsync() => await _syncService.SyncPaquetesAsync();
        public async Task SincronizarEnviosAsync() => await _syncService.SyncEnviosAsync();
        public async Task SincronizarEmpleadosAsync() => await _syncService.SyncEmpleadosAsync();
        public async Task SincronizarContactosAsync() => await _syncService.SyncContactosAsync();
    }
}
    