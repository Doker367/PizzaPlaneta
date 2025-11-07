using Microsoft.EntityFrameworkCore;
using Pizza.Backend.Domain;

namespace Pizza.Backend.Infrastructure.Data;

public partial class MainDbContext : DbContext
{
    public MainDbContext(DbContextOptions<MainDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Calificacione> Calificaciones { get; set; }
    public virtual DbSet<Sucursale> Sucursales { get; set; }
    public virtual DbSet<Tarjeta> Tarjetas { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<Menu> Menus { get; set; }
    public virtual DbSet<Favorito> Favoritos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Calificacione>(entity =>
        {
            entity.ToTable("calificaciones");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comentario).HasColumnName("comentario");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.PedidoId).HasColumnName("pedido_id");
            entity.Property(e => e.Puntuacion).HasColumnName("puntuacion");
            entity.Property(e => e.SucursalId).HasColumnName("sucursal_id");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Sucursal).WithMany(p => p.Calificaciones)
                .HasForeignKey(d => d.SucursalId);

            entity.HasOne(d => d.Usuario).WithMany(p => p.Calificaciones)
                .HasForeignKey(d => d.UsuarioId);
        });

        modelBuilder.Entity<Favorito>(entity =>
        {
            entity.ToTable("favoritos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
            entity.Property(e => e.ProductoId).HasColumnName("producto_id");
            entity.Property(e => e.FechaAgregado).HasColumnName("fecha_agregado").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Usuario)
                .WithMany()
                .HasForeignKey(d => d.UsuarioId);

            entity.HasIndex(e => new { e.UsuarioId, e.ProductoId }).IsUnique();
        });

        modelBuilder.Entity<Sucursale>(entity =>
        {
            entity.ToTable("sucursales");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ciudad).HasColumnName("ciudad");
            entity.Property(e => e.Direccion).HasColumnName("direccion");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.GoogleMapsUrl).HasColumnName("google_maps_url");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
            entity.Property(e => e.Telefono).HasColumnName("telefono");
        });

        modelBuilder.Entity<Tarjeta>(entity =>
        {
            entity.ToTable("tarjetas");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FechaGuardado).HasColumnName("fecha_guardado");
            entity.Property(e => e.StripePaymentMethodId).HasColumnName("stripe_payment_method_id");
            entity.Property(e => e.Last4).HasColumnName("last4");
            entity.Property(e => e.ExpMonth).HasColumnName("exp_month");
            entity.Property(e => e.ExpYear).HasColumnName("exp_year");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Tarjetas)
                .HasForeignKey(d => d.UsuarioId);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasIndex(e => e.Email, "email").IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FechaRegistro).HasColumnName("fecha_registro");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Telefono).HasColumnName("telefono");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.ToTable("menus");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.SucursalId).HasColumnName("sucursal_id");
            entity.Property(e => e.ProductoId).HasColumnName("producto_id");
            entity.Property(e => e.PrecioEspecial).HasColumnName("precio_especial");
            entity.Property(e => e.Disponible).HasColumnName("disponible");

            entity.HasOne(d => d.Sucursal).WithMany(p => p.Menus)
                .HasForeignKey(d => d.SucursalId);

            entity.HasIndex(e => e.ProductoId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}