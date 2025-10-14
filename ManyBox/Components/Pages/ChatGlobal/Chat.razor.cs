using Microsoft.AspNetCore.Components;
using ManyBox.Services;
using ManyBox.Models.Custom;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using System.Linq;

namespace ManyBox.Components.Pages.ChatGlobal
{
    public partial class Chat : ComponentBase, IDisposable
    {
        [Inject] public ChatApiService ApiServiceMensajes { get; set; }
        [Inject] public ChatHubService HubServiceMensajes { get; set; }
        [Inject] public NavigationManager NavigationMensajes { get; set; }
        [Inject] public UserService UserService { get; set; }

        private List<ConversacionModel> conversaciones = new();
        private ConversacionModel? seleccionada;
        private List<MensajeModel> mensajesbox = new();
        private int myUserIdInt;
        private string? errorMessage;
        private string? myRol;
        private string searchSidebar = "";
        private bool mostrarInfoChat = false;
        private bool mostrarFiltro = false;
        private string filtroConversacion = "todos";
        private bool mostrarArchivados = false; // NUEVO: para mostrar archivados

        private long? mensajeEditandoId = null;
        private string mensajeEditandoContenido = string.Empty;

        private MensajeModel? mensajeAResponder = null;
        private long? mensajeAResponderId = null;

        private Dictionary<int, int> mensajesPorConversacion = new(); // convId -> cantidad mensajes del otro usuario

        [Parameter]
        [SupplyParameterFromQuery]
        public int? convId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                myUserIdInt = Preferences.Default.Get("idUsuario", 0);
                if (myUserIdInt == 0)
                {
                    NavigationMensajes.NavigateTo("/login");
                    return;
                }
                var usuario = await UserService.GetUsuarioActualAsync();
                myRol = usuario?.Email == "superadmin@manybox.com" ? "superadmin" : null;
                var convs = await ApiServiceMensajes.GetMisConversacionesAsync();
                var convsExt = new List<ConversacionModelExt>();
                foreach (var c in convs)
                {
                    var mensajes = await ApiServiceMensajes.GetMensajesAsync(c.Id, 1, 1); // Solo el último
                    var nuevos = await ApiServiceMensajes.GetMensajesAsync(c.Id, 1, 50);
                    var nuevosCount = nuevos.Count(m => m.UsuarioId != myUserIdInt && !m.LeidoPorMi);
                    var ultimo = mensajes.FirstOrDefault();
                    convsExt.Add(new ConversacionModelExt
                    {
                        Id = c.Id,
                        Tipo = c.Tipo,
                        Nombre = c.Nombre,
                        FechaCreacion = c.FechaCreacion,
                        Participantes = c.Participantes,
                        Archivada = c.Archivada, // <-- Asegura que se copie el valor de Archivada
                        FechaUltimoMensaje = ultimo?.FechaCreacion,
                        UsuarioUltimoMensajeId = ultimo?.UsuarioId,
                        NuevosMensajes = nuevosCount
                    });
                    // Obtener cantidad de mensajes del otro usuario
                    var counts = await ApiServiceMensajes.GetMensajesPorUsuarioAsync(c.Id);
                    var otro = c.Participantes?.FirstOrDefault(p => p.Id != myUserIdInt);
                    if (otro != null)
                    {
                        var count = counts.FirstOrDefault(x => x.UsuarioId == otro.Id)?.Cantidad ?? 0;
                        mensajesPorConversacion[c.Id] = count;
                    }
                }
                conversaciones = convsExt.Select(c => (ConversacionModel)c).OrderByDescending(c => (c as ConversacionModelExt)?.FechaUltimoMensaje ?? c.FechaCreacion).ToList();
                if (convId.HasValue && conversaciones.Any(c => c.Id == convId.Value))
                {
                    await SeleccionarConversacion(conversaciones.First(c => c.Id == convId.Value));
                }
                // --- SignalR badge update ---
                await HubServiceMensajes.ConnectAsync(NavigationMensajes.BaseUri.TrimEnd('/') + "/chathub");
                HubServiceMensajes.OnActualizarBadgeNoLeidos -= ActualizarBadgeNoLeidosHandler;
                HubServiceMensajes.OnActualizarBadgeNoLeidos += ActualizarBadgeNoLeidosHandler;
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                errorMessage = "No se pudo conectar con el servidor de chat. Verifica tu conexión o vuelve a intentarlo.";
                Console.WriteLine($"HttpRequestException: {ex.Message}");
            }
            catch (System.Exception ex)
            {
                errorMessage = "Ocurrió un error inesperado al cargar el chat.";
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        // Handler para actualizar badge en tiempo real
        private async Task ActualizarBadgeNoLeidosHandler(int conversacionId)
        {
            var counts = await ApiServiceMensajes.GetMensajesPorUsuarioAsync(conversacionId);
            var conv = conversaciones.FirstOrDefault(c => c.Id == conversacionId);
            var otro = conv?.Participantes?.FirstOrDefault(p => p.Id != myUserIdInt);
            if (otro != null)
            {
                var count = counts.FirstOrDefault(x => x.UsuarioId == otro.Id)?.Cantidad ?? 0;
                mensajesPorConversacion[conversacionId] = count;
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task SeleccionarConversacion(ConversacionModel conv)
        {
            // A. Limpia el mensaje de error al inicio
            errorMessage = null;
            StateHasChanged();
            try
            {
                seleccionada = conv;
                mostrarInfoChat = false;
                if (conv is ConversacionModelExt ext)
                    ext.NuevosMensajes = 0;
                // B. Primer try-catch: obtener mensajes
                try
                {
                    await ApiServiceMensajes.MarcarMensajesComoLeidosAsync(conv.Id, myUserIdInt);
                    var mensajes = await ApiServiceMensajes.GetMensajesAsync(conv.Id, 1, 50);
                    mensajesbox = mensajes;
                    // NUEVO: Refresca el contador de mensajes no leídos desde el backend
                    var counts = await ApiServiceMensajes.GetMensajesPorUsuarioAsync(conv.Id);
                    var otro = conv.Participantes?.FirstOrDefault(p => p.Id != myUserIdInt);
                    if (otro != null)
                    {
                        var count = counts.FirstOrDefault(x => x.UsuarioId == otro.Id)?.Cantidad ?? 0;
                        mensajesPorConversacion[conv.Id] = count;
                    }
                }
                catch (System.Exception ex)
                {
                    errorMessage = "No se pudo cargar los mensajes. Verifica tu conexión.";
                    Console.WriteLine($"Error al obtener mensajes: {ex.Message}");
                    // Si no hay mensajes, salimos
                    if (mensajesbox == null || mensajesbox.Count == 0)
                        return;
                }
                // C. Segundo try-catch: conectar a SignalR
                try
                {
                    await HubServiceMensajes.UnirseAConversacion(conv.Id);
                    HubServiceMensajes.OnMensajeRecibido -= MensajeRecibido;
                    HubServiceMensajes.OnMensajeRecibido += MensajeRecibido;
                }
                catch (System.Exception ex)
                {
                    errorMessage = "No se pudo conectar en tiempo real.";
                    Console.WriteLine($"Error SignalR: {ex.Message}");
                }
                // D. Si todo salió bien y hay mensajes, limpia el error
                if (mensajesbox != null && mensajesbox.Count > 0)
                    errorMessage = null;
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                errorMessage = "No se pudo conectar con el servidor de chat. Verifica tu conexión o vuelve a intentarlo.";
                Console.WriteLine($"HttpRequestException: {ex.Message}");
            }
            catch (System.Exception ex)
            {
                errorMessage = "Ocurrió un error inesperado al cargar los mensajes.";
                Console.WriteLine($"Exception: {ex.Message}");
            }
            StateHasChanged();
            // Al leer, notifica a otros (mantén esto para SignalR)
            await HubServiceMensajes.NotificarMensajesNoLeidosAsync(conv.Id);
            await InvokeAsync(StateHasChanged);
        }

        private void ReordenarConversacionesPorUltimoMensaje(int conversacionId, DateTime? fechaUltimoMensaje = null)
        {
            var conv = conversaciones.FirstOrDefault(c => c.Id == conversacionId);
            if (conv != null)
            {
                var fecha = fechaUltimoMensaje ?? mensajesbox.LastOrDefault(m => m.ConversacionId == conversacionId)?.FechaCreacion ?? conv.FechaCreacion;
                conv.FechaCreacion = fecha;
                conversaciones = conversaciones
                    .OrderByDescending(c => c.Id == conversacionId ? fecha : c.FechaCreacion)
                    .ToList();
            }
        }

        private void PrepararResponderMensaje(long mensajeId)
        {
            mensajeAResponderId = mensajeId;
            mensajeAResponder = mensajesbox.FirstOrDefault(m => m.Id == mensajeId);
            StateHasChanged();
        }

        private void CancelarResponderMensaje()
        {
            mensajeAResponder = null;
            mensajeAResponderId = null;
            StateHasChanged();
        }

        private void ConfirmarResponderMensaje(MensajeModel mensaje)
        {
            mensajeAResponder = mensaje;
            mensajeAResponderId = mensaje.Id;
            StateHasChanged();
        }

        private async Task EnviarMensaje((string contenido, string? archivoUrl, string? archivoNombreOriginal) data)
        {
            if (seleccionada == null) return;
            try
            {
                long? replyTo = mensajeAResponderId;
                var tipo = data.archivoUrl != null ? "archivo" : "texto";
                var nuevo = await ApiServiceMensajes.EnviarMensajeAsync(seleccionada.Id, data.contenido, tipo, replyTo, data.archivoUrl, data.archivoNombreOriginal);
                if (nuevo != null)
                {
                    mensajesbox.Add(nuevo);
                    ReordenarConversacionesPorUltimoMensaje(seleccionada.Id, nuevo.FechaCreacion);
                    mensajeAResponder = null;
                    mensajeAResponderId = null;
                    await InvokeAsync(StateHasChanged);
                }
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                errorMessage = "No se pudo enviar el mensaje. Verifica tu conexión.";
                Console.WriteLine($"HttpRequestException: {ex.Message}");
            }
            catch (System.Exception ex)
            {
                errorMessage = "Ocurrió un error inesperado al enviar el mensaje.";
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        private async Task MensajeRecibido(MensajeModel mensaje)
        {
            if (mensaje.ConversacionId == seleccionada?.Id)
            {
                mensajesbox.Add(mensaje);
                ReordenarConversacionesPorUltimoMensaje(mensaje.ConversacionId, mensaje.FechaCreacion);
                // Al recibir mensaje, actualiza el contador si no es mío
                if (mensaje.UsuarioId != myUserIdInt)
                {
                    if (mensajesPorConversacion.ContainsKey(mensaje.ConversacionId))
                        mensajesPorConversacion[mensaje.ConversacionId]++;
                    else
                        mensajesPorConversacion[mensaje.ConversacionId] = 1;
                }
                await InvokeAsync(StateHasChanged);
            }
        }

        public void Dispose()
        {
            // Si quieres desconectar SignalR aquí, puedes hacerlo:
            // await HubServiceMensajes.DisconnectAsync();
        }

        private bool EsSuperAdmin => myRol == "superadmin";

        private async Task EliminarMensaje(long mensajeId)
        {
            var ok = await ApiServiceMensajes.EliminarMensajeAsync(mensajeId);
            if (ok)
            {
                mensajesbox = mensajesbox.Where(m => m.Id != mensajeId).ToList();
                StateHasChanged();
            }
        }

        private async Task EliminarConversacion(int conversacionId)
        {
            if (!EsSuperAdmin) return;
            var ok = await ApiServiceMensajes.EliminarConversacionAsync(conversacionId);
            if (ok)
            {
                conversaciones = conversaciones.Where(c => c.Id != conversacionId).ToList();
                seleccionada = null;
                mensajesbox.Clear();
                StateHasChanged();
            }
        }

        private async Task EliminarChat(int conversacionId)
        {
            if (!EsSuperAdmin) return;
            // Eliminar todos los mensajes primero
            var mensajes = await ApiServiceMensajes.GetMensajesAsync(conversacionId, 1, 1000);
            foreach (var mensaje in mensajes)
            {
                await ApiServiceMensajes.EliminarMensajeAsync(mensaje.Id);
            }
            // Ahora eliminar la conversación
            var ok = await ApiServiceMensajes.EliminarConversacionAsync(conversacionId);
            if (ok)
            {
                conversaciones = conversaciones.Where(c => c.Id != conversacionId).ToList();
                seleccionada = null;
                mensajesbox.Clear();
                StateHasChanged();
            }
        }

        // Sidebar search and filter
        private IEnumerable<ConversacionModel> ConversacionesFiltradas =>
            conversaciones
                .Where(c =>
                    (mostrarArchivados ? c.Archivada : !c.Archivada) &&
                    (filtroConversacion == "todos" || c.Tipo == filtroConversacion)
                    && (string.IsNullOrWhiteSpace(searchSidebar) || GetConversacionNombre(c).Contains(searchSidebar, System.StringComparison.OrdinalIgnoreCase)));

        private void MostrarFiltro() => mostrarFiltro = !mostrarFiltro;
        private void FiltrarConversaciones(string tipo)
        {
            filtroConversacion = tipo;
            mostrarFiltro = false;
        }
        private void MostrarArchivados()
        {
            mostrarArchivados = true;
            mostrarFiltro = false;
            StateHasChanged();
        }
        private void MostrarNoArchivados()
        {
            mostrarArchivados = false;
            mostrarFiltro = false;
            StateHasChanged();
        }
        public void MostrarInfoChat() => mostrarInfoChat = true;
        public void OcultarInfoChat() => mostrarInfoChat = false;

        private async Task EditarMensaje(long mensajeId)
        {
            var mensaje = mensajesbox.FirstOrDefault(m => m.Id == mensajeId);
            if (mensaje != null)
            {
                mensajeEditandoId = mensajeId;
                mensajeEditandoContenido = mensaje.Contenido;
                StateHasChanged();
            }
        }

        private async Task GuardarEdicionMensaje()
        {
            if (mensajeEditandoId.HasValue)
            {
                var mensaje = mensajesbox.FirstOrDefault(m => m.Id == mensajeEditandoId.Value);
                if (mensaje != null && mensaje.Contenido != mensajeEditandoContenido)
                {
                    var ok = await ApiServiceMensajes.EditarMensajeAsync(mensaje.Id, mensajeEditandoContenido);
                    if (ok)
                    {
                        mensaje.Contenido = mensajeEditandoContenido;
                        mensaje.Editado = true;
                    }
                }
                mensajeEditandoId = null;
                mensajeEditandoContenido = string.Empty;
                StateHasChanged();
            }
        }

        private void CancelarEdicionMensaje()
        {
            mensajeEditandoId = null;
            mensajeEditandoContenido = string.Empty;
            StateHasChanged();
        }
    }
}