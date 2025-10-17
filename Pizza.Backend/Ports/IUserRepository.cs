using Pizza.Backend.Domain;

namespace Pizza.Backend.Ports;

public interface IUserRepository
{
    Task<Usuario?> GetUserByEmailAsync(string email);
    Task AddUserAsync(Usuario user);
}
