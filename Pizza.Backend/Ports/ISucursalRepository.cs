using Pizza.Backend.Domain;

namespace Pizza.Backend.Ports;

public interface ISucursalRepository
{
    Task<IEnumerable<Sucursale>> GetAllAsync();
}
