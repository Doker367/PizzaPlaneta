using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza.Backend.Application.DTOs;
using Pizza.Backend.Ports;
using System.Security.Claims;

namespace Pizza.Backend.Adapters;

[Authorize]
[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderDto createOrderDto)
    {
        // Obtener el userId del JWT
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (userId == null)
            return Unauthorized();

        await _orderService.CreateOrder(createOrderDto, userId);

        return Ok(new { message = "Pedido recibido correctamente." });
    }

    [HttpGet]
public async Task<IActionResult> GetOrders()
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
    if (userId == null)
        return Unauthorized();

    var orders = await _orderService.GetOrdersByUser(userId);
    return Ok(orders);
}
}
