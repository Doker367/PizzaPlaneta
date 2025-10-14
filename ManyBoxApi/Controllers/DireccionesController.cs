using ManyBoxApi.DTOs;
using ManyBoxApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManyBoxApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DireccionesController : ControllerBase
    {
        private readonly IDireccionService _direccionService;

        public DireccionesController(IDireccionService direccionService)
        {
            _direccionService = direccionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DireccionDto>>> GetDirecciones()
        {
            var direcciones = await _direccionService.GetDireccionesAsync();
            return Ok(direcciones);
        }

        [HttpPost]
        public async Task<ActionResult<DireccionDto>> CrearDireccion([FromBody] CrearDireccionRequest request)
        {
            var nuevaDireccion = await _direccionService.CreateDireccionAsync(request);
            return CreatedAtAction(nameof(GetDirecciones), new { id = nuevaDireccion.Id }, nuevaDireccion);
        }
    }
}