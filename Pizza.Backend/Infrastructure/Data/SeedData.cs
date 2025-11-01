using Microsoft.EntityFrameworkCore;
using Pizza.Backend.Domain;
using BCrypt.Net;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Pizza.Backend.Infrastructure.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var productsDbContextOptions = serviceProvider.GetRequiredService<DbContextOptions<ProductsDbContext>>();
            var mainDbContextOptions = serviceProvider.GetRequiredService<DbContextOptions<MainDbContext>>();

            using (var context = new ProductsDbContext(productsDbContextOptions))
            {
                if (context.Productos.Any())
                {
                    return; // La base de datos ya ha sido poblada
                }

                var productos = GetProductosFromPdf();
                context.Productos.AddRange(productos);
                context.SaveChanges();
            }

            using (var context = new MainDbContext(mainDbContextOptions))
            {
                // Seed Sucursales
                if (!context.Sucursales.Any())
                {
                    var sucursales = GetSucursalesFromPdf();
                    context.Sucursales.AddRange(sucursales);
                    context.SaveChanges();
                }

                // Seed Menus
                if (!context.Menus.Any())
                {
                    // Volvemos a leer los contextos para obtener los IDs generados por la BD
                    List<Producto> productosFromDb;
                    using (var productsContext = new ProductsDbContext(productsDbContextOptions))
                    {
                        productosFromDb = productsContext.Productos.ToList();
                    }
                    var sucursalesFromDb = context.Sucursales.ToList(); // Get existing sucursales

                    var menus = GetMenusFromPdf(sucursalesFromDb, productosFromDb);
                    context.Menus.AddRange(menus);
                    context.SaveChanges();
                }

                // Seed Admin User
                if (!context.Usuarios.Any(u => u.Email == "blcdoker@gmail.com"))
                {
                    context.Usuarios.Add(new Usuario
                    {
                        Nombre = "Alberto Emiliano",
                        Email = "blcdoker@gmail.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doker367"),
                        Role = "Admin",
                        FechaRegistro = DateTime.UtcNow
                    });
                    context.SaveChanges();
                }
            }
        }

        private static List<Producto> GetProductosFromPdf()
        {
            return new List<Producto>
            {
                // Pizzas
                new Producto { Nombre = "Clásica Margarita", Descripcion = "Queso mozzarella, tomate y albahaca fresca", Precio = 120, Categoria = "Pizzas", Activo = true, Calorias = 1250 },
                new Producto { Nombre = "Mexicana", Descripcion = "Chorizo, jalapeño, cebolla y queso fundido", Precio = 140, Categoria = "Pizzas", Activo = true, Calorias = 1600 },
                new Producto { Nombre = "Hawaiana", Descripcion = "Jamón, piña y queso mozzarella", Precio = 135, Categoria = "Pizzas", Activo = true, Calorias = 1400 },
                new Producto { Nombre = "BBQ Lovers", Descripcion = "Pollo BBQ, cebolla morada y queso cheddar", Precio = 150, Categoria = "Pizzas", Activo = true, Calorias = 1750 },
                new Producto { Nombre = "Cuatro Quesos", Descripcion = "Mozzarella, azul, parmesano y gouda", Precio = 155, Categoria = "Pizzas", Activo = true, Calorias = 1850 },
                new Producto { Nombre = "Suprema", Descripcion = "Pepperoni, jamón, champiñones y aceitunas", Precio = 150, Categoria = "Pizzas", Activo = true, Calorias = 1700 },
                new Producto { Nombre = "Vegetariana", Descripcion = "Pimientos, cebolla, champiñones y elote", Precio = 130, Categoria = "Pizzas", Activo = true, Calorias = 1100 },
                new Producto { Nombre = "BBQ Pollo", Descripcion = "Pollo en salsa BBQ con cebolla morada", Precio = 145, Categoria = "Pizzas", Activo = true, Calorias = 1650 },
                new Producto { Nombre = "Carnívora", Descripcion = "Pepperoni, salchicha italiana, jamón y tocino", Precio = 160, Categoria = "Pizzas", Activo = true, Calorias = 1950 },
                new Producto { Nombre = "Pepperoni Lovers", Descripcion = "Extra pepperoni y queso fundido", Precio = 145, Categoria = "Pizzas", Activo = true, Calorias = 1800 },
                new Producto { Nombre = "Del Mar", Descripcion = "Atún, aceitunas, cebolla morada y orégano", Precio = 155, Categoria = "Pizzas", Activo = true, Calorias = 1300 },
                new Producto { Nombre = "Campestre", Descripcion = "Champiñones, espinaca, tocino y mozzarella", Precio = 150, Categoria = "Pizzas", Activo = true, Calorias = 1550 },
                new Producto { Nombre = "Italiana", Descripcion = "Jamón serrano, aceitunas y albahaca", Precio = 155, Categoria = "Pizzas", Activo = true, Calorias = 1650 },
                new Producto { Nombre = "Tropical", Descripcion = "Piña, jamón y queso manchego", Precio = 140, Categoria = "Pizzas", Activo = true, Calorias = 1450 },
                new Producto { Nombre = "Ranchera", Descripcion = "Chorizo, frijoles, cebolla y queso", Precio = 140, Categoria = "Pizzas", Activo = true, Calorias = 1700 },
                new Producto { Nombre = "Napolitana", Descripcion = "Tomate, albahaca y parmesano", Precio = 135, Categoria = "Pizzas", Activo = true, Calorias = 1200 },
                new Producto { Nombre = "Pollo Alfredo", Descripcion = "Pollo, salsa blanca y champiñones", Precio = 150, Categoria = "Pizzas", Activo = true, Calorias = 1750 },
                new Producto { Nombre = "Pepperoni Clásica", Descripcion = "Pepperoni y queso mozzarella", Precio = 130, Categoria = "Pizzas", Activo = true, Calorias = 1500 },
                new Producto { Nombre = "BBQ Deluxe", Descripcion = "Pollo BBQ, tocino y cebolla", Precio = 155, Categoria = "Pizzas", Activo = true, Calorias = 1800 },
                new Producto { Nombre = "Deluxe", Descripcion = "Pepperoni, jamón, tocino, pimientos", Precio = 160, Categoria = "Pizzas", Activo = true, Calorias = 1900 },

                // Snacks
                new Producto { Nombre = "Palitos de ajo", Descripcion = "Pan artesanal con ajo y mantequilla", Precio = 50, Categoria = "Snacks", Activo = true, Calorias = 450 },
                new Producto { Nombre = "Papas gajo", Descripcion = "Crujientes con especias de la casa", Precio = 45, Categoria = "Snacks", Activo = true, Calorias = 550 },
                new Producto { Nombre = "Dedos de queso", Descripcion = "Empanizados rellenos de mozzarella", Precio = 55, Categoria = "Snacks", Activo = true, Calorias = 600 },
                new Producto { Nombre = "Aros de cebolla", Descripcion = "Crujientes con empanizado casero", Precio = 50, Categoria = "Snacks", Activo = true, Calorias = 500 },
                new Producto { Nombre = "Nachos con guacamole", Descripcion = "Totopos con guacamole y queso fundido", Precio = 65, Categoria = "Snacks", Activo = true, Calorias = 700 },
                new Producto { Nombre = "Nachos con queso", Descripcion = "Totopos con queso fundido", Precio = 60, Categoria = "Snacks", Activo = true, Calorias = 650 },
                new Producto { Nombre = "Alitas BBQ", Descripcion = "6 piezas con salsa BBQ", Precio = 70, Categoria = "Snacks", Activo = true, Calorias = 800 },
                new Producto { Nombre = "Papas fritas", Descripcion = "Corte delgado con sal de ajo", Precio = 45, Categoria = "Snacks", Activo = true, Calorias = 500 },
                new Producto { Nombre = "Mini calzones", Descripcion = "Rellenos de pepperoni y queso", Precio = 60, Categoria = "Snacks", Activo = true, Calorias = 650 },
                new Producto { Nombre = "Nachos supremos", Descripcion = "Con carne molida y queso fundido", Precio = 65, Categoria = "Snacks", Activo = true, Calorias = 850 },
                new Producto { Nombre = "Alitas picantes", Descripcion = "6 piezas con salsa picante", Precio = 70, Categoria = "Snacks", Activo = true, Calorias = 750 },

                // Bebidas
                new Producto { Nombre = "Refresco Coca-Cola", Descripcion = "355ml", Precio = 25, Categoria = "Bebidas", Activo = true, Calorias = 150 },
                new Producto { Nombre = "Refresco Sprite", Descripcion = "355ml", Precio = 25, Categoria = "Bebidas", Activo = true, Calorias = 140 },
                new Producto { Nombre = "Refresco Fanta Naranja", Descripcion = "355ml", Precio = 25, Categoria = "Bebidas", Activo = true, Calorias = 160 },
                new Producto { Nombre = "Agua mineral Peñafiel", Descripcion = "600ml", Precio = 20, Categoria = "Bebidas", Activo = true, Calorias = 0 },
                new Producto { Nombre = "Limonada natural", Descripcion = "500ml con rodajas de limón", Precio = 30, Categoria = "Bebidas", Activo = true, Calorias = 120 },
                new Producto { Nombre = "Refresco Pepsi", Descripcion = "355ml", Precio = 25, Categoria = "Bebidas", Activo = true, Calorias = 150 },
                new Producto { Nombre = "Refresco Mirinda", Descripcion = "355ml sabor naranja", Precio = 25, Categoria = "Bebidas", Activo = true, Calorias = 160 },
                new Producto { Nombre = "Refresco 7Up", Descripcion = "355ml sabor lima-limón", Precio = 25, Categoria = "Bebidas", Activo = true, Calorias = 140 },
                new Producto { Nombre = "Té helado limón", Descripcion = "500ml con hielo", Precio = 28, Categoria = "Bebidas", Activo = true, Calorias = 100 },
                new Producto { Nombre = "Refresco Coca-Cola 600ml", Descripcion = "Botella grande", Precio = 30, Categoria = "Bebidas", Activo = true, Calorias = 250 },
                new Producto { Nombre = "Cerveza artesanal Chiapaneca", Descripcion = "355ml tipo lager", Precio = 45, Categoria = "Bebidas", Activo = true, Calorias = 180 },
                new Producto { Nombre = "Agua natural", Descripcion = "500ml", Precio = 18, Categoria = "Bebidas", Activo = true, Calorias = 0 },
                new Producto { Nombre = "Jugo de mango", Descripcion = "400ml natural", Precio = 28, Categoria = "Bebidas", Activo = true, Calorias = 200 },
                new Producto { Nombre = "Refresco Dr. Pepper", Descripcion = "355ml", Precio = 25, Categoria = "Bebidas", Activo = true, Calorias = 150 },
                new Producto { Nombre = "Refresco Coca-Cola Light", Descripcion = "355ml", Precio = 25, Categoria = "Bebidas", Activo = true, Calorias = 0 },
                new Producto { Nombre = "Té durazno", Descripcion = "500ml", Precio = 28, Categoria = "Bebidas", Activo = true, Calorias = 100 },
                new Producto { Nombre = "Agua mineral", Descripcion = "600ml", Precio = 20, Categoria = "Bebidas", Activo = true, Calorias = 0 },
                new Producto { Nombre = "Jugo de naranja", Descripcion = "400ml natural", Precio = 28, Categoria = "Bebidas", Activo = true, Calorias = 180 },
                new Producto { Nombre = "Agua embotellada", Descripcion = "600ml", Precio = 18, Categoria = "Bebidas", Activo = true, Calorias = 0 },
                new Producto { Nombre = "Té verde", Descripcion = "500ml frío", Precio = 30, Categoria = "Bebidas", Activo = true, Calorias = 90 },
                new Producto { Nombre = "Refresco Fanta Fresa", Descripcion = "355ml", Precio = 25, Categoria = "Bebidas", Activo = true, Calorias = 160 },
                new Producto { Nombre = "Refresco Manzanita", Descripcion = "355ml", Precio = 25, Categoria = "Bebidas", Activo = true, Calorias = 150 },
                new Producto { Nombre = "Jugo natural de piña", Descripcion = "400ml", Precio = 30, Categoria = "Bebidas", Activo = true, Calorias = 190 },
                new Producto { Nombre = "Refresco Fanta Uva", Descripcion = "355ml", Precio = 25, Categoria = "Bebidas", Activo = true, Calorias = 170 },
                new Producto { Nombre = "Cerveza artesanal", Descripcion = "355ml", Precio = 45, Categoria = "Bebidas", Activo = true, Calorias = 180 },

                // Extras
                new Producto { Nombre = "Aderezo ranch", Descripcion = "Cremoso con hierbas y ajo", Precio = 10, Categoria = "Extras", Activo = true, Calorias = 110 },
                new Producto { Nombre = "Extra de queso", Descripcion = "Porción adicional de queso fundido", Precio = 15, Categoria = "Extras", Activo = true, Calorias = 150 },
                new Producto { Nombre = "Orilla rellena", Descripcion = "Masa con queso derretido dentro", Precio = 20, Categoria = "Extras", Activo = true, Calorias = 250 },
                new Producto { Nombre = "Salsa picante", Descripcion = "Salsa roja casera de chile seco", Precio = 10, Categoria = "Extras", Activo = true, Calorias = 30 },
                new Producto { Nombre = "Aderezo BBQ", Descripcion = "Salsa dulce y ahumada", Precio = 12, Categoria = "Extras", Activo = true, Calorias = 80 },
                new Producto { Nombre = "Extra de pepperoni", Descripcion = "Porción adicional de pepperoni", Precio = 15, Categoria = "Extras", Activo = true, Calorias = 100 },
                new Producto { Nombre = "Extra de tocino", Descripcion = "Porción adicional de tocino crujiente", Precio = 15, Categoria = "Extras", Activo = true, Calorias = 120 },
                new Producto { Nombre = "Doble salsa BBQ", Descripcion = "Porción doble de salsa BBQ", Precio = 10, Categoria = "Extras", Activo = true, Calorias = 160 },
                new Producto { Nombre = "Salsa habanera", Descripcion = "Muy picante con chile habanero", Precio = 12, Categoria = "Extras", Activo = true, Calorias = 40 },
                new Producto { Nombre = "Salsa especial", Descripcion = "De jitomate con hierbas finas", Precio = 10, Categoria = "Extras", Activo = true, Calorias = 50 },
                new Producto { Nombre = "Extra de jamón", Descripcion = "Porción adicional de jamón ahumado", Precio = 15, Categoria = "Extras", Activo = true, Calorias = 90 },
                new Producto { Nombre = "Aderezo de ajo", Descripcion = "Salsa cremosa con ajo y especias", Precio = 10, Categoria = "Extras", Activo = true, Calorias = 120 },
                new Producto { Nombre = "Extra de salsa blanca", Descripcion = "Porción adicional de salsa Alfredo", Precio = 12, Categoria = "Extras", Activo = true, Calorias = 100 },
                new Producto { Nombre = "Orilla rellena de queso", Descripcion = "Queso fundido dentro de la masa", Precio = 20, Categoria = "Extras", Activo = true, Calorias = 250 },
                new Producto { Nombre = "Salsa BBQ", Descripcion = "Salsa ahumada de la casa", Precio = 12, Categoria = "Extras", Activo = true, Calorias = 80 },
            }.GroupBy(p => p.Nombre).Select(g => g.First()).ToList(); // Asegurar unicidad
        }

        private static List<Sucursale> GetSucursalesFromPdf()
        {
            return new List<Sucursale>
            {
                new Sucursale { Nombre = "Pizza Planeta Centro", Direccion = "Av. Central Sur 456, Col. Centro", Ciudad = "Tuxtla Gutiérrez", Estado = "Chiapas", Telefono = "961 1234567", GoogleMapsUrl = "https://goo.gl/maps/centro456" },
                new Sucursale { Nombre = "Pizza Planeta Oriente", Direccion = "Blvd. Belisario Domínguez 2100, Col. Terán", Ciudad = "Tuxtla Gutiérrez", Estado = "Chiapas", Telefono = "961 9876543", GoogleMapsUrl = "https://goo.gl/maps/oriente2100" },
                new Sucursale { Nombre = "Pizza Planeta Poniente", Direccion = "Calz. al Sumidero 890, Col. Moctezuma", Ciudad = "Tuxtla Gutiérrez", Estado = "Chiapas", Telefono = "961 3217654", GoogleMapsUrl = "https://goo.gl/maps/poniente890" },
                new Sucursale { Nombre = "Pizza Planeta Norte", Direccion = "Av. Universidad 1220, Col. Mirador", Ciudad = "Tuxtla Gutiérrez", Estado = "Chiapas", Telefono = "961 6547890", GoogleMapsUrl = "https://goo.gl/maps/norte1220" },
                new Sucursale { Nombre = "Pizza Planeta Sur", Direccion = "Calle Central Sur 950, Col. San Roque", Ciudad = "Tuxtla Gutiérrez", Estado = "Chiapas", Telefono = "961 222 3344", GoogleMapsUrl = "https://goo.gl/maps/sur950" },
                new Sucursale { Nombre = "Pizza Planeta Plaza Cristal", Direccion = "Blvd. Ángel Albino Corzo 3000, Plaza Cristal, Local 12", Ciudad = "Tuxtla Gutiérrez", Estado = "Chiapas", Telefono = "961 111 7788", GoogleMapsUrl = "https://goo.gl/maps/plazacristal12" },
                new Sucursale { Nombre = "Pizza Planeta Zona Dorada", Direccion = "Prol. Paseo de la Primavera 890, Col. Zona Dorada", Ciudad = "Tuxtla Gutiérrez", Estado = "Chiapas", Telefono = "961 888 9999", GoogleMapsUrl = "https://goo.gl/maps/zonadorada890" },
            };
        }

        private static List<Menu> GetMenusFromPdf(List<Sucursale> sucursales, List<Producto> productos)
        {
            var sucursalMap = sucursales.ToDictionary(s => s.Nombre, s => s.Id);
            var productoMap = productos.ToDictionary(p => p.Nombre, p => p.Id);

            return new List<Menu>
            {
                // Menu Pizza Planeta Centro
                new Menu { SucursalId = sucursalMap["Pizza Planeta Centro"], ProductoId = productoMap["Clásica Margarita"], PrecioEspecial = 120 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Centro"], ProductoId = productoMap["Mexicana"], PrecioEspecial = 140 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Centro"], ProductoId = productoMap["Hawaiana"], PrecioEspecial = 135 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Centro"], ProductoId = productoMap["BBQ Lovers"], PrecioEspecial = 150 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Centro"], ProductoId = productoMap["Cuatro Quesos"], PrecioEspecial = 155 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Centro"], ProductoId = productoMap["Palitos de ajo"], PrecioEspecial = 50 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Centro"], ProductoId = productoMap["Papas gajo"], PrecioEspecial = 45 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Centro"], ProductoId = productoMap["Dedos de queso"], PrecioEspecial = 55 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Centro"], ProductoId = productoMap["Refresco Coca-Cola"], PrecioEspecial = 25 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Centro"], ProductoId = productoMap["Refresco Sprite"], PrecioEspecial = 25 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Centro"], ProductoId = productoMap["Refresco Fanta Naranja"], PrecioEspecial = 25 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Centro"], ProductoId = productoMap["Agua mineral Peñafiel"], PrecioEspecial = 20 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Centro"], ProductoId = productoMap["Limonada natural"], PrecioEspecial = 30 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Centro"], ProductoId = productoMap["Aderezo ranch"], PrecioEspecial = 10 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Centro"], ProductoId = productoMap["Extra de queso"], PrecioEspecial = 15 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Centro"], ProductoId = productoMap["Orilla rellena"], PrecioEspecial = 20 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Centro"], ProductoId = productoMap["Salsa picante"], PrecioEspecial = 10 },

                // Menu Pizza Planeta Oriente
                new Menu { SucursalId = sucursalMap["Pizza Planeta Oriente"], ProductoId = productoMap["Suprema"], PrecioEspecial = 150 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Oriente"], ProductoId = productoMap["Vegetariana"], PrecioEspecial = 130 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Oriente"], ProductoId = productoMap["BBQ Pollo"], PrecioEspecial = 145 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Oriente"], ProductoId = productoMap["Carnívora"], PrecioEspecial = 155 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Oriente"], ProductoId = productoMap["Dedos de queso"], PrecioEspecial = 55 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Oriente"], ProductoId = productoMap["Aros de cebolla"], PrecioEspecial = 50 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Oriente"], ProductoId = productoMap["Nachos con guacamole"], PrecioEspecial = 65 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Oriente"], ProductoId = productoMap["Refresco Pepsi"], PrecioEspecial = 25 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Oriente"], ProductoId = productoMap["Refresco Mirinda"], PrecioEspecial = 25 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Oriente"], ProductoId = productoMap["Refresco 7Up"], PrecioEspecial = 25 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Oriente"], ProductoId = productoMap["Té helado limón"], PrecioEspecial = 28 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Oriente"], ProductoId = productoMap["Refresco Coca-Cola 600ml"], PrecioEspecial = 30 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Oriente"], ProductoId = productoMap["Salsa picante"], PrecioEspecial = 10 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Oriente"], ProductoId = productoMap["Orilla rellena"], PrecioEspecial = 20 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Oriente"], ProductoId = productoMap["Aderezo BBQ"], PrecioEspecial = 12 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Oriente"], ProductoId = productoMap["Extra de pepperoni"], PrecioEspecial = 15 },

                // Menu Pizza Planeta Poniente
                new Menu { SucursalId = sucursalMap["Pizza Planeta Poniente"], ProductoId = productoMap["Carnívora"], PrecioEspecial = 160 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Poniente"], ProductoId = productoMap["Cuatro Quesos"], PrecioEspecial = 150 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Poniente"], ProductoId = productoMap["Pepperoni Lovers"], PrecioEspecial = 145 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Poniente"], ProductoId = productoMap["Del Mar"], PrecioEspecial = 155 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Poniente"], ProductoId = productoMap["Nachos con queso"], PrecioEspecial = 60 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Poniente"], ProductoId = productoMap["Alitas BBQ"], PrecioEspecial = 70 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Poniente"], ProductoId = productoMap["Papas fritas"], PrecioEspecial = 45 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Poniente"], ProductoId = productoMap["Cerveza artesanal Chiapaneca"], PrecioEspecial = 45 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Poniente"], ProductoId = productoMap["Refresco Coca-Cola"], PrecioEspecial = 30 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Poniente"], ProductoId = productoMap["Refresco Sprite"], PrecioEspecial = 30 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Poniente"], ProductoId = productoMap["Agua natural"], PrecioEspecial = 18 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Poniente"], ProductoId = productoMap["Jugo de mango"], PrecioEspecial = 28 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Poniente"], ProductoId = productoMap["Extra de tocino"], PrecioEspecial = 15 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Poniente"], ProductoId = productoMap["Doble salsa BBQ"], PrecioEspecial = 10 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Poniente"], ProductoId = productoMap["Aderezo ranch"], PrecioEspecial = 10 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Poniente"], ProductoId = productoMap["Salsa habanera"], PrecioEspecial = 12 },

                // Menu Pizza Planeta Norte
                new Menu { SucursalId = sucursalMap["Pizza Planeta Norte"], ProductoId = productoMap["Campestre"], PrecioEspecial = 150 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Norte"], ProductoId = productoMap["Italiana"], PrecioEspecial = 155 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Norte"], ProductoId = productoMap["Tropical"], PrecioEspecial = 140 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Norte"], ProductoId = productoMap["Mini calzones"], PrecioEspecial = 60 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Norte"], ProductoId = productoMap["Aros de cebolla"], PrecioEspecial = 50 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Norte"], ProductoId = productoMap["Refresco Dr. Pepper"], PrecioEspecial = 25 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Norte"], ProductoId = productoMap["Refresco Coca-Cola Light"], PrecioEspecial = 25 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Norte"], ProductoId = productoMap["Té durazno"], PrecioEspecial = 28 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Norte"], ProductoId = productoMap["Limonada natural"], PrecioEspecial = 30 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Norte"], ProductoId = productoMap["Agua mineral"], PrecioEspecial = 20 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Norte"], ProductoId = productoMap["Salsa especial"], PrecioEspecial = 10 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Norte"], ProductoId = productoMap["Extra de jamón"], PrecioEspecial = 15 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Norte"], ProductoId = productoMap["Aderezo de ajo"], PrecioEspecial = 10 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Norte"], ProductoId = productoMap["Orilla rellena"], PrecioEspecial = 20 },

                // Menu Pizza Planeta Sur
                new Menu { SucursalId = sucursalMap["Pizza Planeta Sur"], ProductoId = productoMap["Ranchera"], PrecioEspecial = 140 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Sur"], ProductoId = productoMap["Napolitana"], PrecioEspecial = 135 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Sur"], ProductoId = productoMap["Pollo Alfredo"], PrecioEspecial = 150 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Sur"], ProductoId = productoMap["Papas gajo"], PrecioEspecial = 50 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Sur"], ProductoId = productoMap["Dedos de queso"], PrecioEspecial = 55 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Sur"], ProductoId = productoMap["Refresco Coca-Cola"], PrecioEspecial = 30 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Sur"], ProductoId = productoMap["Refresco Sprite"], PrecioEspecial = 30 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Sur"], ProductoId = productoMap["Jugo de naranja"], PrecioEspecial = 28 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Sur"], ProductoId = productoMap["Agua embotellada"], PrecioEspecial = 18 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Sur"], ProductoId = productoMap["Té verde"], PrecioEspecial = 30 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Sur"], ProductoId = productoMap["Extra de salsa blanca"], PrecioEspecial = 12 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Sur"], ProductoId = productoMap["Aderezo de ajo"], PrecioEspecial = 10 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Sur"], ProductoId = productoMap["Extra de jamón"], PrecioEspecial = 15 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Sur"], ProductoId = productoMap["Orilla rellena"], PrecioEspecial = 20 },

                // Menu Pizza Planeta Plaza Cristal
                new Menu { SucursalId = sucursalMap["Pizza Planeta Plaza Cristal"], ProductoId = productoMap["Pepperoni Clásica"], PrecioEspecial = 130 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Plaza Cristal"], ProductoId = productoMap["BBQ Deluxe"], PrecioEspecial = 155 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Plaza Cristal"], ProductoId = productoMap["Vegetariana"], PrecioEspecial = 125 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Plaza Cristal"], ProductoId = productoMap["Alitas picantes"], PrecioEspecial = 70 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Plaza Cristal"], ProductoId = productoMap["Papas fritas"], PrecioEspecial = 45 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Plaza Cristal"], ProductoId = productoMap["Refresco Fanta Fresa"], PrecioEspecial = 25 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Plaza Cristal"], ProductoId = productoMap["Refresco Manzanita"], PrecioEspecial = 25 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Plaza Cristal"], ProductoId = productoMap["Agua natural"], PrecioEspecial = 18 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Plaza Cristal"], ProductoId = productoMap["Jugo natural de piña"], PrecioEspecial = 30 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Plaza Cristal"], ProductoId = productoMap["Refresco 7Up"], PrecioEspecial = 25 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Plaza Cristal"], ProductoId = productoMap["Orilla rellena de queso"], PrecioEspecial = 20 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Plaza Cristal"], ProductoId = productoMap["Aderezo BBQ"], PrecioEspecial = 10 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Plaza Cristal"], ProductoId = productoMap["Extra de tocino"], PrecioEspecial = 15 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Plaza Cristal"], ProductoId = productoMap["Salsa picante"], PrecioEspecial = 10 },

                // Menu Pizza Planeta Zona Dorada
                new Menu { SucursalId = sucursalMap["Pizza Planeta Zona Dorada"], ProductoId = productoMap["Deluxe"], PrecioEspecial = 160 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Zona Dorada"], ProductoId = productoMap["BBQ Lovers"], PrecioEspecial = 150 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Zona Dorada"], ProductoId = productoMap["Cuatro Quesos"], PrecioEspecial = 155 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Zona Dorada"], ProductoId = productoMap["Nachos supremos"], PrecioEspecial = 65 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Zona Dorada"], ProductoId = productoMap["Papas gajo"], PrecioEspecial = 45 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Zona Dorada"], ProductoId = productoMap["Refresco Coca-Cola"], PrecioEspecial = 25 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Zona Dorada"], ProductoId = productoMap["Refresco Sprite"], PrecioEspecial = 25 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Zona Dorada"], ProductoId = productoMap["Refresco Fanta Uva"], PrecioEspecial = 25 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Zona Dorada"], ProductoId = productoMap["Cerveza artesanal"], PrecioEspecial = 45 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Zona Dorada"], ProductoId = productoMap["Agua mineral"], PrecioEspecial = 20 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Zona Dorada"], ProductoId = productoMap["Extra de queso"], PrecioEspecial = 15 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Zona Dorada"], ProductoId = productoMap["Aderezo ranch"], PrecioEspecial = 10 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Zona Dorada"], ProductoId = productoMap["Orilla rellena"], PrecioEspecial = 20 },
                new Menu { SucursalId = sucursalMap["Pizza Planeta Zona Dorada"], ProductoId = productoMap["Salsa BBQ"], PrecioEspecial = 12 },
            };
        }
    }
}