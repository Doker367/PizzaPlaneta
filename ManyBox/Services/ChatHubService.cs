using Microsoft.AspNetCore.SignalR.Client;
using ManyBox.Models.Custom;
using System;
using System.Threading.Tasks;

namespace ManyBox.Services
{
    public class ChatHubService
    {
        private HubConnection? _hubConnection;
        public event Func<MensajeModel, Task>? OnMensajeRecibido;
        public event Func<int, Task>? OnActualizarBadgeNoLeidos;

        public async Task ConnectAsync(string hubUrl)
        {
            if (_hubConnection != null) return;
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<MensajeModel>("RecibirMensaje", async (mensaje) =>
            {
                if (OnMensajeRecibido != null)
                    await OnMensajeRecibido.Invoke(mensaje);
            });

            _hubConnection.On<int>("ActualizarBadgeNoLeidos", async (conversacionId) =>
            {
                if (OnActualizarBadgeNoLeidos != null)
                    await OnActualizarBadgeNoLeidos.Invoke(conversacionId);
            });

            await _hubConnection.StartAsync();
        }

        public async Task UnirseAConversacion(int conversacionId)
        {
            if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
                await _hubConnection.InvokeAsync("UnirseAConversacion", conversacionId);
        }

        public async Task SendMensajeAsync(int conversacionId, string contenido, string tipo)
        {
            if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
                await _hubConnection.InvokeAsync("EnviarMensaje", conversacionId, contenido, tipo);
        }

        public async Task NotificarMensajesNoLeidosAsync(int conversacionId)
        {
            if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
                await _hubConnection.InvokeAsync("NotificarMensajesNoLeidos", conversacionId);
        }

        public async Task DisconnectAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }
        }
    }
}