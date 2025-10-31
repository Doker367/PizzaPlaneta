using Microsoft.EntityFrameworkCore;
using Pizza.Backend.Domain;

namespace Pizza.Backend.Infrastructure.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var productsDbContext = new ProductsDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ProductsDbContext>>()))
            {
                if (!productsDbContext.Productos.Any())
                {
                    productsDbContext.Productos.AddRange(
                        new Producto { Nombre = "Pizza Hawaiana", Descripcion = "Jamón y piña.", Precio = 150.00m, Categoria = "Clásicas", Activo = true, Calorias = 1200 },
                        new Producto { Nombre = "Pizza Pepperoni", Descripcion = "Pepperoni y queso.", Precio = 160.00m, Categoria = "Clásicas", Activo = true, Calorias = 1500 },
                        new Producto { Nombre = "Pizza Mexicana", Descripcion = "Chorizo, jalapeños, y frijoles.", Precio = 170.00m, Categoria = "Especialidades", Activo = true, Calorias = 1800 }
                    );
                    productsDbContext.SaveChanges();
                }
            }

            using (var mainDbContext = new MainDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<MainDbContext>>()))
            {
                if (!mainDbContext.Sucursales.Any())
                {
                    mainDbContext.Sucursales.AddRange(
                        new Sucursale { Nombre = "Pizza Planeta Central", Direccion = "Av. Central 123", Ciudad = "Tuxtla Gutiérrez", Estado = "Chiapas", Telefono = "961-555-0101", GoogleMapsUrl = "https://maps.google.com/?q=Pizza+Planeta+Central" },
                        new Sucursale { Nombre = "Pizza Planeta Oriente", Direccion = "Blvd. Oriente 456", Ciudad = "Tuxtla Gutiérrez", Estado = "Chiapas", Telefono = "961-555-0102", GoogleMapsUrl = "https://maps.google.com/?q=Pizza+Planeta+Oriente" },
                        new Sucursale { Nombre = "Pizza Planeta Poniente", Direccion = "Calzada Poniente 789", Ciudad = "Tuxtla Gutiérrez", Estado = "Chiapas", Telefono = "961-555-0103", GoogleMapsUrl = "https://maps.google.com/?q=Pizza+Planeta+Poniente" }
                    );
                    mainDbContext.SaveChanges();
                }

                if (!mainDbContext.Menus.Any())
                {
                    mainDbContext.Menus.AddRange(
                        // Sucursal 1 (Central)
                        new Menu { SucursalId = 1, ProductoId = 1, Disponible = true },
                        new Menu { SucursalId = 1, ProductoId = 2, Disponible = true, PrecioEspecial = 155.00m }, // Precio especial

                        // Sucursal 2 (Oriente)
                        new Menu { SucursalId = 2, ProductoId = 2, Disponible = true },
                        new Menu { SucursalId = 2, ProductoId = 3, Disponible = true },

                        // Sucursal 3 (Poniente)
                        new Menu { SucursalId = 3, ProductoId = 1, Disponible = true },
                        new Menu { SucursalId = 3, ProductoId = 2, Disponible = true },
                        new Menu { SucursalId = 3, ProductoId = 3, Disponible = true }
                    );
                    mainDbContext.SaveChanges();
                }
            }
        }
    }
}