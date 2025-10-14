using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ManyBox.Services;
using ManyBox.Utils;
using System.Timers;
using Microsoft.JSInterop;

namespace ManyBox.Components.Layout
{
    public partial class NavMenu : ComponentBase, IDisposable
    {
        [Inject] public ChatApiService ChatApiService { get; set; }
        [Inject] public NotificacionesService NotificacionesService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        public int UnreadMessagesCount { get; set; }
        public int UnreadNotificacionesCount { get; set; }
        private System.Timers.Timer? _refreshTimer;

        public bool IsLoggedIn => SessionState.IsLoggedIn;
        public string Usuario => SessionState.Usuario;
        public string Rol => SessionState.Rol;

        protected override async Task OnInitializedAsync()
        {
            await RefrescarBadgesAsync();
            _refreshTimer = new System.Timers.Timer(10000); // refresca cada 10s
            _refreshTimer.Elapsed += async (s, e) => await RefrescarBadgesAsync();
            _refreshTimer.Start();
        }

        private async Task RefrescarBadgesAsync()
        {
            if (SessionState.IsLoggedIn)
            {
                UnreadMessagesCount = await ChatApiService.ObtenerMensajesNoLeidosCount();
                UnreadNotificacionesCount = await NotificacionesService.ObtenerNotificacionesNoLeidasCount();
                await InvokeAsync(StateHasChanged);
            }
        }

        public void Dispose()
        {
            _refreshTimer?.Dispose();
        }

        // Métodos para click en Chat/Notificaciones
        public async Task OnChatClick()
        {
            await RefrescarBadgesAsync();
        }
        public async Task OnNotificacionesClick()
        {
            await RefrescarBadgesAsync();
        }

        public string GetRolString()
        {
            return (SessionState.Rol ?? "usuario").ToLower();
        }

        public void CerrarSesion()
        {
            SessionState.IsLoggedIn = false;
            SessionState.Usuario = string.Empty;
            SessionState.Rol = string.Empty;
            SessionState.IdUsuario = 0;
            SessionState.IdSucursal = 0;
            SessionState.Token = string.Empty;
            NavigationManager.NavigateTo("/login", true);
        }
    }
}
