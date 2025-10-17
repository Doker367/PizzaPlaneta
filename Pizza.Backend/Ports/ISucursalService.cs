using Pizza.Backend.Domain;

namespace Pizza.Backend.Ports;

public interface ISucursalService
{
    Task<IEnumerable<Sucursale>> GetAllAsync();
}
