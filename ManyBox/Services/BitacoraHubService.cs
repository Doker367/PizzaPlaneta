using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace ManyBox.Services
{
    public class BitacoraHubService : IAsyncDisposable
    {
        private HubConnection? _hubConnection;
        public event Func<Task>? OnBitacoraActualizada;

        public async Task ConnectAsync(string hubUrl, int empleadoId)
        {
            if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
                return;
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On("BitacoraActualizada", async () =>
            {
                if (OnBitacoraActualizada != null)
                    await OnBitacoraActualizada.Invoke();
            });

            await _hubConnection.StartAsync();
            await _hubConnection.InvokeAsync("JoinBitacoraGroup", empleadoId);
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
            }
        }
    }
}
