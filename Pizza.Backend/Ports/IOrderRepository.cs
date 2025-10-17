using Pizza.Backend.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pizza.Backend.Ports;

public interface IOrderRepository
{
    Task<bool> CreateOrder(Pedido order, List<DetallePedido> orderDetails);
    Task<List<Pedido>> GetOrdersByUser(int userId);
}
