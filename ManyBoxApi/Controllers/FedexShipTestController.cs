using Microsoft.AspNetCore.Mvc;

namespace ManyBoxApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FedexShipTestController : ControllerBase
    {
        // Este endpoint solo devuelve el JSON recibido para revisión
        [HttpPost("test")]
        public IActionResult TestReceiveFedexJson([FromBody] object fedexJson)
        {
            // Aquí podrías loguear el cuerpo recibido, luego devolverlo
            return Ok(fedexJson);
        }
    }
}
