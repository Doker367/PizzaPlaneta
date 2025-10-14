using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ManyBoxApi.Hubs
{
    public class ChatHub : Hub
    {
        public async Task UnirseAConversacion(int conversacionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversacionId.ToString());
        }

        public async Task EnviarMensaje(int conversacionId, string contenido, string tipo)
        {
            // Aquí podrías guardar el mensaje en la BD si quieres.
            await Clients.Group(conversacionId.ToString()).SendAsync("RecibirMensaje", new
            {
                ConversacionId = conversacionId,
                Contenido = contenido,
                Tipo = tipo,
                FechaCreacion = DateTime.UtcNow
            });
        }

        // NUEVO: Notificar a todos los usuarios de la conversación que hay mensajes no leídos
        public async Task NotificarMensajesNoLeidos(int conversacionId)
        {
            await Clients.Group(conversacionId.ToString()).SendAsync("ActualizarBadgeNoLeidos", conversacionId);
        }
    }
}
