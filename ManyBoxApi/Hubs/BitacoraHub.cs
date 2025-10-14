using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ManyBoxApi.Hubs
{
    public class BitacoraHub : Hub
    {
        // Llama a este m�todo cuando cambie el estado de un env�o relevante para la bit�cora
        public async Task NotificarActualizacionBitacora(int empleadoId)
        {
            await Clients.Group($"bitacora_{empleadoId}").SendAsync("BitacoraActualizada");
        }

        public override async Task OnConnectedAsync()
        {
            // El cliente debe llamar a JoinBitacoraGroup(empleadoId) despu�s de conectar
            await base.OnConnectedAsync();
        }

        public async Task JoinBitacoraGroup(int empleadoId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"bitacora_{empleadoId}");
        }
        public async Task LeaveBitacoraGroup(int empleadoId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"bitacora_{empleadoId}");
        }
    }
}
