using System.Net.Http.Json;
using ManyBox.Models.Custom;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ManyBox.Utils;
using System.Net.Http;

namespace ManyBox.Services
{
    public class ChatApiService
    {
        private readonly HttpClient _http;

        public ChatApiService(HttpClient http)
        {
            _http = http;
        }

        // Este método es para obtener las conversaciones
        public async Task<List<ConversacionModel>> GetMisConversacionesAsync()
        {
            var result = await _http.GetFromJsonAsync<List<ConversacionModel>>("api/conversaciones");
            return result ?? new List<ConversacionModel>();
        }

        // Este método es para obtener los mensajes de una conversación
        public async Task<List<MensajeModel>> GetMensajesAsync(int conversacionId, int pagina = 1, int pageSize = 50)
        {
            var url = $"api/conversaciones/{conversacionId}/mensajes?pagina={pagina}&pageSize={pageSize}";
            try
            {
                var result = await _http.GetFromJsonAsync<List<MensajeModel>>(url);
                return result ?? new List<MensajeModel>();
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                Console.WriteLine($"Error Http al obtener mensajes: {ex.Message}");
                return new List<MensajeModel>();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error inesperado al obtener mensajes: {ex.Message}");
                return new List<MensajeModel>();
            }
        }

        // Este método es para enviar un mensaje
        public async Task<MensajeModel?> EnviarMensajeAsync(int conversacionId, string contenido, string tipo, long? replyTo = null, string? archivoUrl = null, string? archivoNombreOriginal = null)
        {
            var payload = new { contenido, tipo, replyToId = replyTo, archivoUrl, archivoNombreOriginal };
            var res = await _http.PostAsJsonAsync($"api/conversaciones/{conversacionId}/mensajes", payload);
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<MensajeModel>();
        }

        // SUBIDA DE ARCHIVOS: Word, Excel, PowerPoint, PDF, TXT, etc.
        public async Task<(string? url, string? nombreOriginal)> SubirArchivoAsync(Stream fileStream, string fileName)
        {
            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(fileStream);
            // Opcional: hints de tipo mime
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(GetMimeType(fileName));
            content.Add(streamContent, "archivo", fileName);
            var res = await _http.PostAsync("api/archivos/upload", content);
            if (!res.IsSuccessStatusCode) return (null, null);
            var obj = await res.Content.ReadFromJsonAsync<ArchivoUploadResponse>();
            return (obj?.Url, obj?.NombreOriginal);
        }

        private static string GetMimeType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".csv" => "text/csv",
                ".rtf" => "application/rtf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                // imágenes
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".bmp" => "image/bmp",
                ".tif" or ".tiff" => "image/tiff",
                ".svg" => "image/svg+xml",
                // audio
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                // video
                ".mp4" => "video/mp4",
                ".mov" => "video/quicktime",
                // comprimidos
                ".zip" => "application/zip",
                ".rar" => "application/vnd.rar",
                ".7z" => "application/x-7z-compressed",
                _ => "application/octet-stream"
            };
        }

        // Este método es para obtener la lista de usuarios/contactos
        public async Task<List<UsuarioContactoVM>> GetContactosAsync()
        {
            // Intenta obtener todos los usuarios si es superadmin/admin, si no, usa endpoint público o uno especial para chofer
            try
            {
                var result = await _http.GetFromJsonAsync<List<UsuarioContactoVM>>("api/usuarios");
                return result ?? new List<UsuarioContactoVM>();
            }
            catch (System.Net.Http.HttpRequestException)
            {
                // Si no tiene permisos, intenta obtener solo los usuarios básicos (por ejemplo, endpoint público)
                try
                {
                    var result = await _http.GetFromJsonAsync<List<UsuarioContactoVM>>("api/usuarios/contactos");
                    return result ?? new List<UsuarioContactoVM>();
                }
                catch
                {
                    return new List<UsuarioContactoVM>();
                }
            }
        }

        // Este método es para crear una nueva conversación (1 a 1 o grupo)
        public async Task<ConversacionModel?> CrearConversacionAsync(CrearConversacionRequest request)
        {
            // Forzar que solo se envíen valores válidos para 'Tipo'
            if (request.Tipo != "directo" && request.Tipo != "grupo")
                request.Tipo = "directo";
            var res = await _http.PostAsJsonAsync("api/conversaciones", request);
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<ConversacionModel>();
        }

        // NUEVO: Buscar si ya existe un chat directo entre dos usuarios
        public async Task<ConversacionModel?> BuscarConversacionDirectaAsync(int usuario1Id, int usuario2Id)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<ConversacionModel>($"api/conversaciones/buscar-directo?usuario1={usuario1Id}&usuario2={usuario2Id}");
                return result;
            }
            catch
            {
                return null;
            }
        }

        // Buscar contactos con filtro de nombre, apellido y sucursal
        public async Task<List<UsuarioContactoVM>> BuscarContactosAsync(string? search = null, int? sucursalId = null)
        {
            var url = "api/usuarios/buscar-contactos";
            var query = new List<string>();
            if (!string.IsNullOrWhiteSpace(search)) query.Add($"search={Uri.EscapeDataString(search)}");
            if (sucursalId.HasValue) query.Add($"sucursalId={sucursalId.Value}");
            if (query.Count > 0) url += "?" + string.Join("&", query);
            var result = await _http.GetFromJsonAsync<List<UsuarioContactoVM>>(url);
            return result ?? new List<UsuarioContactoVM>();
        }

        // Eliminar mensaje (solo admin/superadmin)
        public async Task<bool> EliminarMensajeAsync(long mensajeId)
        {
            var res = await _http.DeleteAsync($"api/mensajes/{mensajeId}");
            return res.IsSuccessStatusCode;
        }

        // Eliminar conversación (solo admin/superadmin)
        public async Task<bool> EliminarConversacionAsync(int conversacionId)
        {
            var res = await _http.DeleteAsync($"api/conversaciones/{conversacionId}");
            return res.IsSuccessStatusCode;
        }

        // Marcar todos los mensajes de una conversación como leídos para el usuario actuel
        public async Task MarcarMensajesComoLeidosAsync(int conversacionId, int usuarioId)
        {
            var res = await _http.PostAsJsonAsync($"api/conversaciones/{conversacionId}/marcar-leidos", new { usuarioId });
            res.EnsureSuccessStatusCode();
        }

        // Obtener lista de sucursales
        public async Task<List<SucursalModel>> GetSucursalesAsync()
        {
            var result = await _http.GetFromJsonAsync<List<SucursalModel>>("api/sucursales");
            return result ?? new List<SucursalModel>();
        }

        // NUEVO: Método para editar mensaje
        public async Task<bool> EditarMensajeAsync(long mensajeId, string nuevoContenido)
        {
            var payload = new { contenido = nuevoContenido };
            var res = await _http.PutAsJsonAsync($"api/mensajes/{mensajeId}", payload);
            return res.IsSuccessStatusCode;
        }

        // NUEVO: Método para archivar conversación
        public async Task<bool> ArchivarConversacionAsync(int conversacionId)
        {
            var res = await _http.PatchAsync($"api/conversaciones/{conversacionId}/archivar", null);
            return res.IsSuccessStatusCode;
        }

        // NUEVO: Método para desarchivar conversación
        public async Task<bool> DesarchivarConversacionAsync(int conversacionId)
        {
            var res = await _http.PatchAsync($"api/conversaciones/{conversacionId}/desarchivar", null);
            return res.IsSuccessStatusCode;
        }

        // Cambiar nombre de grupo
        public async Task<bool> CambiarNombreGrupoAsync(int conversacionId, string nuevoNombre)
        {
            var payload = new { nombre = nuevoNombre };
            var res = await _http.PatchAsJsonAsync($"api/conversaciones/{conversacionId}/cambiar-nombre", payload);
            return res.IsSuccessStatusCode;
        }

        // Agregar participantes a grupo
        public async Task<bool> AgregarParticipantesGrupoAsync(int conversacionId, List<int> nuevosParticipantesIds)
        {
            var res = await _http.PostAsJsonAsync($"api/conversaciones/{conversacionId}/agregar-participantes", nuevosParticipantesIds);
            return res.IsSuccessStatusCode;
        }

        // Eliminar participantes de grupo
        public async Task<bool> EliminarParticipantesGrupoAsync(int conversacionId, List<int> participantesIds)
        {
            var res = await _http.PostAsJsonAsync($"api/conversaciones/{conversacionId}/eliminar-participantes", participantesIds);
            return res.IsSuccessStatusCode;
        }

        // Verifica si el usuario actual es admin en la conversación
        public async Task<bool> EsAdminEnConversacionAsync(int conversacionId)
        {
            var res = await _http.GetFromJsonAsync<EsAdminResponse>($"api/conversaciones/{conversacionId}/es-admin");
            return res?.esAdmin ?? false;
        }
        private class EsAdminResponse { public bool esAdmin { get; set; } }

        // Asignar/quitar administradores de grupo
        public async Task<bool> AsignarAdministradoresGrupoAsync(int conversacionId, List<int> adminUserIds)
        {
            var payload = new { usuarioIds = adminUserIds };
            var res = await _http.PatchAsJsonAsync($"api/conversaciones/{conversacionId}/asignar-admins", payload);
            return res.IsSuccessStatusCode;
        }

        // Obtener ids de administradores en una conversación
        public async Task<List<int>> GetAdminIdsEnConversacionAsync(int conversacionId)
        {
            var convs = await GetMisConversacionesAsync();
            var conv = convs.FirstOrDefault(c => c.Id == conversacionId);
            if (conv == null || conv.Participantes == null)
                return new List<int>();
            // Obtener los ids de los administradores desde la API (llamar endpoint si existe, o asumir que el primero es el creador y admin)
            // Si tienes un endpoint mejor, cámbialo aquí
            var response = await _http.GetAsync($"api/conversaciones/{conversacionId}/admins");
            if (response.IsSuccessStatusCode)
            {
                var ids = await response.Content.ReadFromJsonAsync<List<int>>();
                return ids ?? new List<int>();
            }
            // Fallback: solo el primero es admin
            return conv.Participantes.Take(1).Select(p => p.Id).ToList();
        }

        // Obtener el id del creador de una conversación
        public async Task<int> GetCreadorIdEnConversacionAsync(int conversacionId)
        {
            var response = await _http.GetAsync($"api/conversaciones/{conversacionId}/creador");
            if (response.IsSuccessStatusCode)
            {
                var obj = await response.Content.ReadFromJsonAsync<CreadorIdResponse>();
                return obj?.creadorId ?? 0;
            }
            return 0;
        }
        private class CreadorIdResponse { public int creadorId { get; set; } }

        // Obtener estados de un mensaje (enviado/leído por usuario)
        public async Task<List<MensajeEstadoModel>> GetEstadosMensajeAsync(long mensajeId)
        {
            var estados = await _http.GetFromJsonAsync<List<MensajeEstadoModel>>($"api/mensajes/{mensajeId}/estados");
            return estados ?? new List<MensajeEstadoModel>();
        }

        // Obtener cantidad de mensajes no leídos para el usuario actual
        public async Task<int> ObtenerMensajesNoLeidosCount()
        {
            int usuarioId = SessionState.IdUsuario;
            var result = await _http.GetFromJsonAsync<int>($"api/usuarios/{usuarioId}/mensajes/no-leidos/count");
            return result;
        }

        // Marcar todos los mensajes como leídos para el usuario actual
        public async Task MarcarMensajesComoLeidos()
        {
            int usuarioId = SessionState.IdUsuario;
            var res = await _http.PutAsync($"api/usuarios/{usuarioId}/mensajes/marcar-todos-leidos", null);
            res.EnsureSuccessStatusCode();
        }

        // Obtener conteo de mensajes enviados por usuario en una conversación
        public async Task<List<UsuarioMensajesCount>> GetMensajesPorUsuarioAsync(int conversacionId)
        {
            var result = await _http.GetFromJsonAsync<List<UsuarioMensajesCount>>($"api/conversaciones/{conversacionId}/mensajes-por-usuario");
            return result ?? new List<UsuarioMensajesCount>();
        }

        public class UsuarioMensajesCount
        {
            public int UsuarioId { get; set; }
            public int Cantidad { get; set; }
        }
    }
}