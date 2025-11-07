using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pizza.Backend.Application.DTOs;
using Pizza.Backend.Domain;
using Pizza.Backend.Infrastructure.Data;
using Pizza.Backend.Ports;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pizza.Backend.Application;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITarjetaRepository _tarjetaRepository;
    private readonly MainDbContext _mainDbContext; // Injected for Sucursal and Calificaciones
    private readonly string _stripeSecretKey;


    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, MainDbContext mainDbContext, IUserRepository userRepository, ITarjetaRepository tarjetaRepository, IConfiguration configuration)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _mainDbContext = mainDbContext;
        _userRepository = userRepository;
        _tarjetaRepository = tarjetaRepository;
        _stripeSecretKey = configuration["Stripe:SecretKey"];
    }

    public async Task<bool> CreateOrder(CreateOrderDto createOrderDto, string userId)
    {
        var user = await _userRepository.GetUserByIdAsync(int.Parse(userId));
        if (user == null) throw new Exception("Usuario no encontrado.");

        var productIds = createOrderDto.Items.Select(x => x.ProductoId).ToList();
        var productos = await _productRepository.GetProductsByIds(productIds);
        if (productos.Count != productIds.Count)
            throw new Exception("Uno o más productos no existen.");

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

        var pedido = new Pedido
        {
            UsuarioId = int.Parse(userId),
            SucursalId = createOrderDto.SucursalId,
            Total = total,
            Fecha = DateTime.UtcNow,
            MetodoPago = createOrderDto.MetodoPago,
            TarjetaId = createOrderDto.TarjetaId
        };

        if (createOrderDto.MetodoPago == "Efectivo")
        {
            pedido.Estado = "Pendiente";
            await _orderRepository.CreateOrder(pedido, detalles);
            return true;
        }
        else if (createOrderDto.MetodoPago == "Tarjeta")
        {
            if (!createOrderDto.TarjetaId.HasValue)
            {
                throw new Exception("Se requiere una tarjeta para el pago con tarjeta.");
            }

            var tarjeta = await _tarjetaRepository.GetByIdAsync(createOrderDto.TarjetaId.Value);
            if (tarjeta == null || tarjeta.UsuarioId != user.Id) throw new Exception("Tarjeta no válida.");

            pedido.Estado = "Pendiente de Pago";
            await _orderRepository.CreateOrder(pedido, detalles);

            try
            {
                StripeConfiguration.ApiKey = _stripeSecretKey;

                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(total * 100), // Amount in cents
                    Currency = "mxn",
                    Customer = user.StripeCustomerId,
                    PaymentMethod = tarjeta.StripePaymentMethodId,
                    OffSession = true,
                    Confirm = true,
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                if (paymentIntent.Status == "succeeded")
                {
                    pedido.Estado = "Pagado";
                    await _orderRepository.UpdateOrder(pedido);
                    return true;
                }
                else
                {
                    pedido.Estado = "Pago Fallido";
                    await _orderRepository.UpdateOrder(pedido);
                    return false;
                }
            }
            catch (StripeException e)
            {
                pedido.Estado = "Pago Fallido";
                await _orderRepository.UpdateOrder(pedido);
                // Log the error
                Console.WriteLine(e.StripeError.Message);
                return false;
            }
        }
        else
        {
            throw new Exception("Método de pago no válido.");
        }
    }

    public async Task<List<OrderDetailDto>> GetOrdersByUser(string userId)
    {
        var pedidos = await _orderRepository.GetOrdersByUser(int.Parse(userId));
        if (!pedidos.Any())
        {
            return new List<OrderDetailDto>();
        }

        // --- Efficient Data Loading --- 

        // 1. Collect all unique IDs from the orders
        var sucursalIds = pedidos.Select(p => p.SucursalId).Distinct().ToList();
        var pedidoIds = pedidos.Select(p => p.Id).ToList();
        var productIds = pedidos.SelectMany(p => p.DetallePedidos.Select(d => d.ProductoId)).Distinct().ToList();

        // 2. Fetch all related data in single queries
        var sucursales = await _mainDbContext.Sucursales
            .Where(s => sucursalIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id);

        var productos = await _productRepository.GetProductsByIds(productIds)
            .ContinueWith(t => t.Result.ToDictionary(p => p.Id));

        var calificaciones = await _mainDbContext.Calificaciones
            .Where(c => pedidoIds.Contains(c.PedidoId))
            .ToDictionaryAsync(c => c.PedidoId);

        // 3. Build DTOs in memory
        var result = pedidos.Select(pedido =>
        {
            sucursales.TryGetValue(pedido.SucursalId, out var sucursal);
            calificaciones.TryGetValue(pedido.Id, out var calificacion);

            var productItems = pedido.DetallePedidos.Select(d =>
            {
                productos.TryGetValue(d.ProductoId, out var producto);
                return new ProductItemDto
                {
                    Nombre = producto?.Nombre ?? "Producto no encontrado",
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Calorias = producto?.Calorias
                };
            }).ToList();

            return new OrderDetailDto
            {
                PedidoId = pedido.Id,
                Sucursal = sucursal?.Nombre ?? "Sucursal no encontrada",
                Fecha = pedido.Fecha ?? DateTime.MinValue,
                Estado = pedido.Estado ?? string.Empty,
                Total = pedido.Total,
                Calificacion = calificacion?.Puntuacion,
                Comentario = calificacion?.Comentario,
                Productos = productItems
            };
        }).ToList();

        return result.OrderByDescending(x => x.Fecha).ToList();
    }

    public async Task<bool> UpdateOrderStatus(int orderId, string status)
    {
        var pedido = await _orderRepository.GetOrderById(orderId);
        if (pedido == null) return false;

        pedido.Estado = status;
        await _orderRepository.UpdateOrder(pedido);

        return true;
    }
}
