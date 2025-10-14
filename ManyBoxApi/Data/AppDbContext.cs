using Microsoft.EntityFrameworkCore;
using ManyBoxApi.Models;

namespace ManyBoxApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<NotificacionEntrega> NotificacionEntregas { get; set; }
        public DbSet<NotificacionDestinatario> NotificacionDestinatarios { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Direccion> Direcciones { get; set; }
        public DbSet<Envio> Envios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Conversacion> Conversaciones { get; set; }
        public DbSet<ConversacionParticipante> ConversacionParticipantes { get; set; }
        public DbSet<MensajeChat> MensajesChat { get; set; }
        public DbSet<MensajeLeido> MensajesLeidos { get; set; }
        public DbSet<Remitente> Remitentes { get; set; }
        public DbSet<Destinatario> Destinatarios { get; set; }
        public DbSet<DetalleContenido> DetalleContenidos { get; set; }
        public DbSet<SeguimientoPaquete> SeguimientoPaquete { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(u => u.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(u => u.Rol)
                      .WithMany(r => r.Usuarios)
                      .HasForeignKey(u => u.RolId);
            });

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.Property(r => r.Nombre).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<DetalleVenta>()
                .HasKey(dv => new { dv.VentaId, dv.ProductoId });
            modelBuilder.Entity<DetalleVenta>()
                .HasOne(dv => dv.Venta)
                .WithMany(v => v.DetalleVentas)
                .HasForeignKey(dv => dv.VentaId);
            modelBuilder.Entity<DetalleVenta>()
                .HasOne(dv => dv.Producto)
                .WithMany(p => p.DetalleProducto)
                .HasForeignKey(dv => dv.ProductoId);

            modelBuilder.Entity<Envio>(entity =>
            {
                // No configurar Fecha ni DireccionId
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Apellido).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Correo).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Telefono).IsRequired().HasMaxLength(20);
            });

            modelBuilder.Entity<NotificacionEntrega>(entity =>
            {
                entity.HasOne(e => e.Notificacion)
                      .WithMany(n => n.Entregas)
                      .HasForeignKey(e => e.NotificacionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<NotificacionDestinatario>(entity =>
            {
                entity.HasOne(d => d.Notificacion)
                      .WithMany(n => n.Destinatarios)
                      .HasForeignKey(d => d.NotificacionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Empleado>()
                .HasOne(e => e.Sucursal)
                .WithMany()
                .HasForeignKey(e => e.SucursalId);

            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Remitente)
                .WithMany()
                .HasForeignKey(v => v.Remitente_Id)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Destinatario)
                .WithMany()
                .HasForeignKey(v => v.Destinatario_Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Elimina relación con Venta
            // modelBuilder.Entity<SeguimientoPaquete>()
            //     .HasOne(s => s.Venta)
            //     .WithMany(v => v.Seguimientos)
            //     .HasForeignKey(s => s.VentaId)
            //     .OnDelete(DeleteBehavior.Cascade);
            // Relación correcta: SeguimientoPaquete -> Envio
            modelBuilder.Entity<SeguimientoPaquete>()
                .HasOne(s => s.Envio)
                .WithMany(e => e.Seguimientos)
                .HasForeignKey(s => s.EnvioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}