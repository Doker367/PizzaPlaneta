using Pizza.Backend.Application.DTOs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Pizza.Backend.Ports;

public interface IOrderService
{
    Task<bool> CreateOrder(CreateOrderDto createOrderDto, string userId);
    Task<List<OrderDetailDto>> GetOrdersByUser(string userId);
    Task<bool> UpdateOrderStatus(int orderId, string status);
}
