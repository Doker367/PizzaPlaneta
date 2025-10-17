using Microsoft.EntityFrameworkCore;
using Pizza.Backend.Domain;
using Pizza.Backend.Infrastructure.Data;
using Pizza.Backend.Ports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pizza.Backend.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly PizzaPlanetaContext _context;

    public OrderRepository(PizzaPlanetaContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateOrder(Pedido order, List<DetallePedido> orderDetails)
{
    await _context.Pedidos.AddAsync(order);
    await _context.SaveChangesAsync(); // Guarda el pedido y genera el Id

    // Asigna el id generado a cada detalle
    foreach (var detalle in orderDetails)
    {
        detalle.PedidoId = order.Id;
    }
    await _context.DetallePedidos.AddRangeAsync(orderDetails);
    await _context.SaveChangesAsync();

    return true;
}


public async Task<List<Pedido>> GetOrdersByUser(int userId)
{
    return await _context.Pedidos
        .Include(p => p.Sucursal)
        .Include(p => p.DetallePedidos)
            .ThenInclude(d => d.Producto)
        .Include(p => p.Calificaciones)
        .Where(p => p.UsuarioId == userId)
        .OrderByDescending(p => p.Fecha)
        .ToListAsync();
}

}
