using Pizza.Backend.Application.DTOs;
using Pizza.Backend.Domain;
using Pizza.Backend.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pizza.Backend.Application
{
    public class FavoritoService : IFavoritoService
    {
        private readonly IFavoritoRepository _favoritoRepository;
        private readonly IProductRepository _productRepository;

        public FavoritoService(IFavoritoRepository favoritoRepository, IProductRepository productRepository)
        {
            _favoritoRepository = favoritoRepository;
            _productRepository = productRepository;
        }

        public async Task<List<ProductItemDto>> GetFavoritosAsync(int userId)
        {
            var favoritos = await _favoritoRepository.GetByUserIdAsync(userId);
            if (!favoritos.Any())
            {
                return new List<ProductItemDto>();
            }

            var productoIds = favoritos.Select(f => f.ProductoId).ToList();
            var productos = await _productRepository.GetProductsByIds(productoIds);

            return productos.Select(p => new ProductItemDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                PrecioUnitario = p.Precio,
                Calorias = p.Calorias,
                Categoria = p.Categoria
            }).ToList();
        }

        public async Task AddFavoritoAsync(int userId, int productoId)
        {
            // Verificar que el producto existe
            var producto = await _productRepository.GetByIdAsync(productoId);
            if (producto == null)
            {
                throw new Exception("El producto no existe.");
            }

            // Verificar que no sea ya un favorito
            var existente = await _favoritoRepository.GetByUserAndProductAsync(userId, productoId);
            if (existente != null)
            {
                return; // Ya es un favorito, no hacer nada
            }

            var nuevoFavorito = new Favorito
            {
                UsuarioId = userId,
                ProductoId = productoId,
                FechaAgregado = DateTime.UtcNow
            };

            await _favoritoRepository.AddAsync(nuevoFavorito);
            await _favoritoRepository.SaveChangesAsync();
        }

        public async Task RemoveFavoritoAsync(int userId, int productoId)
        {
            var favorito = await _favoritoRepository.GetByUserAndProductAsync(userId, productoId);
            if (favorito != null)
            {
                _favoritoRepository.Delete(favorito);
                await _favoritoRepository.SaveChangesAsync();
            }
        }
    }
}
