
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza.Backend.Application.DTOs;
using Pizza.Backend.Ports;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pizza.Backend.Adapters
{
    [Authorize]
    [ApiController]
    [Route("api/tarjetas")]
    public class TarjetaController : ControllerBase
    {
        private readonly ITarjetaService _tarjetaService;

        public TarjetaController(ITarjetaService tarjetaService)
        {
            _tarjetaService = tarjetaService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new Exception("User ID no encontrado en el token.");
            }
            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCards()
        {
            try
            {
                var userId = GetUserId();
                var tarjetas = await _tarjetaService.GetUserCardsAsync(userId);
                return Ok(tarjetas);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCard([FromBody] AddTarjetaDto tarjetaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = GetUserId();
                var nuevaTarjeta = await _tarjetaService.AddCardAsync(userId, tarjetaDto);
                return CreatedAtAction(nameof(GetUserCards), new { id = nuevaTarjeta.Id }, nuevaTarjeta);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{tarjetaId}")]
        public async Task<IActionResult> DeleteCard(int tarjetaId)
        {
            try
            {
                var userId = GetUserId();
                await _tarjetaService.DeleteCardAsync(userId, tarjetaId);
                return NoContent(); // 204 No Content es una respuesta estándar para un delete exitoso
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message); // 403 Forbidden si intentan borrar una tarjeta ajena
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
