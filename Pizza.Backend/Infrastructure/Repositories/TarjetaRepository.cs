
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pizza.Backend.Domain;
using Pizza.Backend.Infrastructure.Data;
using Pizza.Backend.Ports;

namespace Pizza.Backend.Infrastructure.Repositories
{
    public class TarjetaRepository : ITarjetaRepository
    {
        private readonly PizzaPlanetaContext _context;

        public TarjetaRepository(PizzaPlanetaContext context)
        {
            _context = context;
        }

        public async Task<Tarjeta?> GetByIdAsync(int tarjetaId)
        {
            return await _context.Tarjetas.FindAsync(tarjetaId);
        }

        public async Task<List<Tarjeta>> GetByUserIdAsync(int userId)
        {
            return await _context.Tarjetas
                .Where(t => t.UsuarioId == userId)
                .ToListAsync();
        }

        public async Task AddAsync(Tarjeta tarjeta)
        {
            await _context.Tarjetas.AddAsync(tarjeta);
        }

        public void Delete(Tarjeta tarjeta)
        {
            _context.Tarjetas.Remove(tarjeta);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
