using Pizza.Backend.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pizza.Backend.Ports
{
    public interface IFavoritoRepository
    {
        Task<List<Favorito>> GetByUserIdAsync(int userId);
        Task<Favorito?> GetByUserAndProductAsync(int userId, int productoId);
        Task AddAsync(Favorito favorito);
        void Delete(Favorito favorito);
        Task<int> SaveChangesAsync();
    }
}
