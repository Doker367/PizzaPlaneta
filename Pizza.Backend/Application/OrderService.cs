using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pizza.Backend.Application.DTOs;
using Pizza.Backend.Domain;
using Pizza.Backend.Ports;

namespace Pizza.Backend.Application;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<bool> CreateOrder(CreateOrderDto createOrderDto, string userId)
    {
        // 1. Obtener los productos y validar existencia
        var productIds = createOrderDto.Items.Select(x => x.ProductoId).ToList();
        var productos = await _productRepository.GetProductsByIds(productIds);
        if (productos.Count != productIds.Count)
            throw new Exception("Uno o más productos no existen.");

        // 2. Calcular el total y armar DetallePedido
        var detalles = new List<DetallePedido>();
        decimal total = 0;
        foreach (var item in createOrderDto.Items)
        {
            var producto = productos.First(x => x.Id == item.ProductoId);
            detalles.Add(new DetallePedido
            {
                ProductoId = producto.Id,
                Cantidad = item.Cantidad,
                PrecioUnitario = producto.Precio
            });
            total += producto.Precio * item.Cantidad;
        }

        // 3. Crear Pedido
        var pedido = new Pedido
        {
            UsuarioId = int.Parse(userId),
            SucursalId = createOrderDto.SucursalId,
            Estado = "Pendiente",
            Total = total,
            Fecha = DateTime.UtcNow
        };

        // 4. Guardar en base de datos
        await _orderRepository.CreateOrder(pedido, detalles);

        return true;
    }
    // Implementación del método para historial de pedidos
    public async Task<List<OrderDetailDto>> GetOrdersByUser(string userId)
    {
        var pedidos = await _orderRepository.GetOrdersByUser(int.Parse(userId));
        var result = new List<OrderDetailDto>();
        foreach (var pedido in pedidos)
        {
            var detalle = new OrderDetailDto
            {
                PedidoId = pedido.Id,
                Sucursal = pedido.Sucursal?.Nombre ?? string.Empty,
                Fecha = pedido.Fecha ?? DateTime.MinValue,
                Estado = pedido.Estado ?? string.Empty,
                Total = pedido.Total,
                Calificacion = pedido.Calificaciones.FirstOrDefault()?.Puntuacion,
                Comentario = pedido.Calificaciones.FirstOrDefault()?.Comentario
            };
            detalle.Productos = pedido.DetallePedidos.Select(d => new ProductItemDto
            {
                Nombre = d.Producto?.Nombre ?? string.Empty,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario
            }).ToList();
            result.Add(detalle);
        }
        return result.OrderByDescending(x => x.Fecha).ToList();
    }
}
