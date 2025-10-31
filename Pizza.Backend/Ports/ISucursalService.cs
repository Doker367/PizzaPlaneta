using Pizza.Backend.Application.DTOs;
using Pizza.Backend.Domain;

namespace Pizza.Backend.Ports;

public interface ISucursalService
{
    Task<IEnumerable<Sucursale>> GetAllAsync();
    Task<IEnumerable<MenuItemDto>> GetMenuBySucursalId(int sucursalId);
}
