
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DotNetEnv;
using System;
using System.IO;

namespace Pizza.Backend.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MainDbContext>
    {
        public MainDbContext CreateDbContext(string[] args)
        {
            // Intenta cargar el archivo .env desde la raíz del proyecto
            var path = Directory.GetCurrentDirectory();
            var envFile = Path.Combine(path, ".env");
            if(!File.Exists(envFile)) {
                 // Si no se encuentra, intenta subir dos niveles (común en herramientas de EF)
                 path = Path.Combine(path, "..", "..");
                 envFile = Path.Combine(path, ".env");
            }
            
            Env.Load(envFile);

            var optionsBuilder = new DbContextOptionsBuilder<MainDbContext>();
            var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión POSTGRES_CONNECTION_STRING no está configurada. Asegúrate de que tu archivo .env esté en la raíz del proyecto.");
            }

            optionsBuilder.UseNpgsql(connectionString);

            return new MainDbContext(optionsBuilder.Options);
        }
    }
}
