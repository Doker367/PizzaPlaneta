
using Pizza.Backend.Domain;

namespace Pizza.Backend.Ports;

public interface IMenuRepository
{
    Task<IEnumerable<Menu>> GetAllAsync();
}
