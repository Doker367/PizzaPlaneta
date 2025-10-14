using ManyBoxApi.Data;
using ManyBoxApi.Helpers;
using ManyBoxApi.Models;
using System.Security.Cryptography;
using System.Text;

public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        if (!context.Roles.Any())
        {
            var roles = new List<Rol>
            {
                new Rol { Nombre = "SuperAdmin", Permisos = "{\"accesoTotal\":true}" },
                new Rol { Nombre = "Admin", Permisos = "{\"gestionUsuarios\":true}" },
                new Rol { Nombre = "Recepción", Permisos = "{\"crearDocumentos\":true}" },
                new Rol { Nombre = "Ventas", Permisos = "{\"consultarCostos\":true}" }
            };

            context.Roles.AddRange(roles);
            context.SaveChanges();
        }

        if (!context.Usuarios.Any())
        {
            var superAdmin = new Usuario
            {
                Username = "superadmin",
                PasswordHash = PasswordHelper.CreateHash("Admin123"),
                RolId = context.Roles.First(r => r.Nombre == "SuperAdmin").Id
            };
            context.Usuarios.Add(superAdmin);
            context.SaveChanges();
        }
    }
}
