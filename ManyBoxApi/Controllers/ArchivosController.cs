using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ManyBoxApi.DTOs;
using System.IO;
using Microsoft.Net.Http.Headers;

namespace ManyBoxApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ArchivosController : ControllerBase
    {
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            // Documentos
            ".pdf", ".txt", ".csv", ".rtf",
            ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
            // Imágenes
            ".png", ".jpg", ".jpeg", ".gif", ".webp", ".bmp", ".tif", ".tiff", ".svg",
            // Audio
            ".mp3", ".wav",
            // Video
            ".mp4", ".mov",
            // Comprimidos
            ".zip", ".rar", ".7z"
        };

        // 50 MB por defecto
        private const long MaxFileSize = 50L * 1024 * 1024;

        [HttpPost("upload")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ArchivoUploadResponse>> Upload([FromForm] IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("No se recibió archivo.");
            if (archivo.Length > MaxFileSize)
                return BadRequest($"El archivo excede el tamaño máximo permitido de {MaxFileSize / (1024 * 1024)} MB.");

            var extension = Path.GetExtension(archivo.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
                return BadRequest("Tipo de archivo no permitido. Extensiones permitidas: PDF, TXT, CSV, RTF, DOC/DOCX, XLS/XLSX, PPT/PPTX, PNG/JPG/JPEG/GIF/WEBP/BMP/TIF/TIFF/SVG, MP3/WAV, MP4/MOV, ZIP/RAR/7Z.");

            // Estructura de guardado: /Uploads/Chat/yyyy/MM
            var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Chat", DateTime.UtcNow.Year.ToString(), DateTime.UtcNow.Month.ToString("00"));
            Directory.CreateDirectory(uploadsRoot);

            var safeFileName = Path.GetFileNameWithoutExtension(archivo.FileName);
            foreach (var c in Path.GetInvalidFileNameChars())
                safeFileName = safeFileName.Replace(c, '_');
            var storedFileName = $"{safeFileName}_{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(uploadsRoot, storedFileName);

            await using (var stream = System.IO.File.Create(fullPath))
            {
                await archivo.CopyToAsync(stream);
            }

            // Construir URL pública usando el middleware de archivos estáticos (mapeado en Program.cs)
            var urlPath = $"/Uploads/Chat/{DateTime.UtcNow.Year}/{DateTime.UtcNow.Month:00}/{storedFileName}";
            var response = new ArchivoUploadResponse
            {
                Url = urlPath,
                NombreOriginal = archivo.FileName
            };
            return Ok(response);
        }
    }
}
