
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza.Backend.Application.DTOs;
using Pizza.Backend.Ports;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pizza.Backend.Adapters
{
    [Authorize]
    [ApiController]
    [Route("api/carrito")]
    public class CarritoController : ControllerBase
    {
        private readonly ICarritoService _carritoService;

        public CarritoController(ICarritoService carritoService)
        {
            _carritoService = carritoService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new System.Exception("User ID no encontrado en el token.");
            }
            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var userId = GetUserId();
                var cart = await _carritoService.GetCartByUserIdAsync(userId);
                return Ok(cart);
            }
            catch (System.Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItemToCart([FromBody] AddItemToCartDto itemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = GetUserId();
                var cart = await _carritoService.AddItemToCartAsync(userId, itemDto);
                return Ok(cart);
            }
            catch (System.Exception ex)
            {
                // Considerar logging del error
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("items/{productoId}")]
        public async Task<IActionResult> RemoveItemFromCart(int productoId)
        {
            try
            {
                var userId = GetUserId();
                var cart = await _carritoService.RemoveItemFromCartAsync(userId, productoId);
                return Ok(cart);
            }
            catch (System.Exception ex)
            {
                // Considerar logging del error
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
