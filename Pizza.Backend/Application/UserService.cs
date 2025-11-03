using Pizza.Backend.Application.DTOs;
using Pizza.Backend.Domain;
using Pizza.Backend.Ports;
using System.Linq;
using System.Threading.Tasks;

namespace Pizza.Backend.Application
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITarjetaService _tarjetaService;
        private readonly IOrderService _orderService;

        public UserService(IUserRepository userRepository, ITarjetaService tarjetaService, IOrderService orderService)
        {
            _userRepository = userRepository;
            _tarjetaService = tarjetaService;
            _orderService = orderService;
        }

        public async Task<UserProfileDto> GetUserProfileAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new System.Exception("Usuario no encontrado.");
            }

            var tarjetas = await _tarjetaService.GetUserCardsAsync(userId);
            var historial = await _orderService.GetOrdersByUser(userId.ToString());

            var profileDto = new UserProfileDto
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Email = user.Email,
                Telefono = user.Telefono,
                Tarjetas = tarjetas.ToList(),
                HistorialPedidos = historial
            };

            return profileDto;
        }

        public async Task<Usuario> UpdateUserProfileAsync(int userId, UpdateUserProfileDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new System.Exception("Usuario no encontrado.");
            }

            user.Nombre = dto.Nombre ?? user.Nombre;
            user.Telefono = dto.Telefono ?? user.Telefono;

            await _userRepository.UpdateUserAsync(user);

            return user;
        }
    }
}
