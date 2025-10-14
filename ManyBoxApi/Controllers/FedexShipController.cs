using Microsoft.AspNetCore.Mvc;
using ManyBoxApi.Models;
using ManyBoxApi.Services;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ManyBoxApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FedexShipController : ControllerBase
    {
        private readonly FedexShipService _fedexShipService;

        public FedexShipController(FedexShipService fedexShipService)
        {
            _fedexShipService = fedexShipService;
        }

        [HttpPost("create-shipment")]
        public async Task<IActionResult> CreateShipment([FromBody] object request)
        {
            var (content, contentType, filePath) = await _fedexShipService.CreateShipmentAsync(request);

            // Si el contentType es PDF o ZPL, regresa como archivo y la ruta local
            if (contentType.Contains("pdf") || contentType.Contains("zpl"))
            {
                return Ok(new
                {
                    message = "PDF recibido y guardado correctamente.",
                    filePath,
                    downloadUrl = filePath != null ? Url.Content($"~/ArchivosFedex/{Path.GetFileName(filePath)}") : null
                });
            }

            // Si es JSON, deserializa y regresa la respuesta tipada
            if (contentType.Contains("json"))
            {
                var jsonString = Encoding.UTF8.GetString(content);
                try
                {
                    var fedexResponse = System.Text.Json.JsonSerializer.Deserialize<object>(jsonString);
                    return Ok(fedexResponse);
                }
                catch (System.Text.Json.JsonException ex)
                {
                    return BadRequest(new
                    {
                        message = "Error al deserializar la respuesta JSON de Fedex.",
                        error = ex.Message,
                        json = jsonString
                    });
                }
            }

            // Si el tipo de contenido no es esperado, devolver mensaje informativo y preview binario
            return BadRequest(new
            {
                message = "La respuesta de Fedex no es reconocida como JSON, PDF ni ZPL. Es probable que sea un archivo binario.",
                contentType,
                preview = BitConverter.ToString(content.Take(20).ToArray())
            });
        }
    }
}