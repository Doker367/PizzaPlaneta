using Pizza.Backend.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pizza.Backend.Ports
{
    public interface IFavoritoService
    {
        Task<List<ProductItemDto>> GetFavoritosAsync(int userId);
        Task AddFavoritoAsync(int userId, int productoId);
        Task RemoveFavoritoAsync(int userId, int productoId);
    }
}
