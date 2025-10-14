using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ManyBoxApi.Hubs
{
    [Authorize]
    public class NotificacionesHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var usuarioId = Context.GetHttpContext()?.Request.Query["usuarioId"].ToString();
            if (!string.IsNullOrEmpty(usuarioId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, usuarioId);
            }
            await base.OnConnectedAsync();
        }

        public async Task EnviarNotificacion(int usuarioId, object notificacion)
        {
            await Clients.Group(usuarioId.ToString())
                .SendAsync("RecibirNotificacion", notificacion);
        }

        public async Task JoinUserGroup(int userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        public async Task LeaveUserGroup(int userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
    }
}