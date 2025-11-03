using Pizza.Backend.Domain;

namespace Pizza.Backend.Ports;

public interface IUserRepository
{
    Task<Usuario?> GetUserByEmailAsync(string email);
    Task<Usuario?> GetUserByIdAsync(int userId);
    Task AddUserAsync(Usuario user);
    Task UpdateUserAsync(Usuario user);
}
