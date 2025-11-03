using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza.Backend.Ports;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pizza.Backend.Adapters
{
    [Authorize]
    [ApiController]
    [Route("api/favoritos")]
    public class FavoritoController : ControllerBase
    {
        private readonly IFavoritoService _favoritoService;

        public FavoritoController(IFavoritoService favoritoService)
        {
            _favoritoService = favoritoService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new System.Exception("ID de usuario no encontrado en el token.");
            }
            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> GetFavoritos()
        {
            var userId = GetUserId();
            var favoritos = await _favoritoService.GetFavoritosAsync(userId);
            return Ok(favoritos);
        }

        [HttpPost("{productoId}")]
        public async Task<IActionResult> AddFavorito(int productoId)
        {
            try
            {
                var userId = GetUserId();
                await _favoritoService.AddFavoritoAsync(userId, productoId);
                return Ok(new { message = "Producto añadido a favoritos." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{productoId}")]
        public async Task<IActionResult> RemoveFavorito(int productoId)
        {
            var userId = GetUserId();
            await _favoritoService.RemoveFavoritoAsync(userId, productoId);
            return NoContent(); // 204 No Content es una respuesta estándar para un delete exitoso
        }
    }
}
