using ManyBoxApi.Models;

namespace ManyBoxApi.Repositories
{
    public interface IDireccionRepository
    {
        Task<IEnumerable<Direccion>> GetDireccionesAsync();
        Task<Direccion> CrearDireccionAsync(Direccion direccion);
    }
}
