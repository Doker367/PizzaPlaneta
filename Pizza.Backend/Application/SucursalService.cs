using Pizza.Backend.Domain;
using Pizza.Backend.Ports;

namespace Pizza.Backend.Application;

public class SucursalService : ISucursalService
{
    private readonly ISucursalRepository _sucursalRepository;

    public SucursalService(ISucursalRepository sucursalRepository)
    {
        _sucursalRepository = sucursalRepository;
    }

    public async Task<IEnumerable<Sucursale>> GetAllAsync()
    {
        return await _sucursalRepository.GetAllAsync();
    }
}
