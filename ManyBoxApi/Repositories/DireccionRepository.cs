using ManyBoxApi.Data;
using ManyBoxApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ManyBoxApi.Repositories
{
    public class DireccionRepository : IDireccionRepository
    {
        private readonly AppDbContext _context;

        public DireccionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Direccion>> GetDireccionesAsync()
        {
            return await _context.Set<Direccion>().AsNoTracking().ToListAsync();
        }

        public async Task<Direccion> CrearDireccionAsync(Direccion direccion)
        {
            _context.Set<Direccion>().Add(direccion);
            await _context.SaveChangesAsync();
            return direccion;
        }
    }
}
