using Pizza.Backend.Application.DTOs;

namespace Pizza.Backend.Ports;

public interface IAuthService
{
    Task RegisterAsync(RegisterUserDto registerUserDto);
    Task<LoginResponseDto> LoginAsync(LoginUserDto loginUserDto);
}
