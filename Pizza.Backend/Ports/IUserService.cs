using Pizza.Backend.Application.DTOs;
using Pizza.Backend.Domain;
using System.Threading.Tasks;

namespace Pizza.Backend.Ports
{
    public interface IUserService
    {
        Task<UserProfileDto> GetUserProfileAsync(int userId);
        Task<Usuario> UpdateUserProfileAsync(int userId, UpdateUserProfileDto dto);
    }
}
