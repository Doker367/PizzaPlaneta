using Pizza.Backend.Domain;
using Pizza.Backend.Application.DTOs;

namespace Pizza.Backend.Ports;

public interface IMenuService
{
    Task<IEnumerable<MenuDto>> GetAllAsync();
}
