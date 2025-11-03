using Microsoft.EntityFrameworkCore;
using Pizza.Backend.Domain;
using Pizza.Backend.Infrastructure.Data;
using Pizza.Backend.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pizza.Backend.Infrastructure.Repositories
{
    public class FavoritoRepository : IFavoritoRepository
    {
        private readonly MainDbContext _context;

        public FavoritoRepository(MainDbContext context)
        {
            _context = context;
        }

        public async Task<List<Favorito>> GetByUserIdAsync(int userId)
        {
            return await _context.Favoritos
                .Where(f => f.UsuarioId == userId)
                .ToListAsync();
        }

        public async Task<Favorito?> GetByUserAndProductAsync(int userId, int productoId)
        {
            return await _context.Favoritos
                .FirstOrDefaultAsync(f => f.UsuarioId == userId && f.ProductoId == productoId);
        }

        public async Task AddAsync(Favorito favorito)
        {
            await _context.Favoritos.AddAsync(favorito);
        }

        public void Delete(Favorito favorito)
        {
            _context.Favoritos.Remove(favorito);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
