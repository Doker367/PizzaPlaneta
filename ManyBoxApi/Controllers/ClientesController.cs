using ManyBoxApi.DTOs;
using ManyBoxApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManyBoxApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        // GET: api/clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes(
            [FromQuery] string? nombre = null,
            [FromQuery] string? correo = null,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 12)
        {
            var clientes = await _clienteService.GetClientesAsync(nombre, correo, skip, take);
            return Ok(clientes);
        }

        // GET: api/clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> GetCliente(int id)
        {
            var cliente = await _clienteService.GetClienteByIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return Ok(cliente);
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<ActionResult<ClienteDto>> CrearCliente([FromBody] CrearClienteRequest request)
        {
            var nuevoCliente = await _clienteService.CreateClienteAsync(request);
            return CreatedAtAction(nameof(GetCliente), new { id = nuevoCliente.Id }, nuevoCliente);
        }

        // DELETE: api/clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var result = await _clienteService.DeleteClienteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}