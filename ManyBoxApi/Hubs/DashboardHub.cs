using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ManyBoxApi.Hubs
{
    [Authorize]
    public class DashboardHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public Task Ping() => Clients.Caller.SendAsync("pong");

        // Método opcional para que servicios/controladores emitan actualizaciones
        public static class Methods
        {
            public const string DashboardUpdate = "dashboardUpdate";
        }
    }
}
