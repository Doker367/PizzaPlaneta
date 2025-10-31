using Microsoft.EntityFrameworkCore;
using Pizza.Backend.Domain;

namespace Pizza.Backend.Infrastructure.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Pedido> Pedidos { get; set; }
    public virtual DbSet<DetallePedido> DetallePedidos { get; set; }
    public virtual DbSet<HistorialEstadoPedido> HistorialEstadoPedidos { get; set; }
    public virtual DbSet<Carrito> Carritos { get; set; }
    public virtual DbSet<CarritoItem> CarritoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("detalle_pedido");
            entity.HasIndex(e => e.OfertaId, "oferta_id");
            entity.HasIndex(e => e.PedidoId, "pedido_id");
            entity.HasIndex(e => e.ProductoId, "producto_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.OfertaId).HasColumnName("oferta_id");
            entity.Property(e => e.PedidoId).HasColumnName("pedido_id");
            entity.Property(e => e.PrecioUnitario).HasPrecision(9, 2).HasColumnName("precio_unitario");
            entity.Property(e => e.ProductoId).HasColumnName("producto_id");

            // Relationship to Pedido (within this context)
            entity.HasOne(d => d.Pedido).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("detalle_pedido_ibfk_1");

            // Relationships to Oferta and Producto (in other DBs) are removed.
        });

        modelBuilder.Entity<HistorialEstadoPedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("historial_estado_pedido");
            entity.HasIndex(e => e.PedidoId, "pedido_id");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado).HasMaxLength(20).HasColumnName("estado");
            entity.Property(e => e.Fecha).HasDefaultValueSql("CURRENT_TIMESTAMP").HasColumnType("datetime").HasColumnName("fecha");
            entity.Property(e => e.PedidoId).HasColumnName("pedido_id");

            entity.HasOne(d => d.Pedido).WithMany(p => p.HistorialEstadoPedidos)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("historial_estado_pedido_ibfk_1");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("pedidos");
            entity.HasIndex(e => e.SucursalId, "sucursal_id");
            entity.HasIndex(e => e.TarjetaId, "tarjeta_id");
            entity.HasIndex(e => e.UsuarioId, "usuario_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado).HasMaxLength(20).HasColumnName("estado");
            entity.Property(e => e.Fecha).HasDefaultValueSql("CURRENT_TIMESTAMP").HasColumnType("datetime").HasColumnName("fecha");
            entity.Property(e => e.SucursalId).HasColumnName("sucursal_id");
            entity.Property(e => e.TarjetaId).HasColumnName("tarjeta_id");
            entity.Property(e => e.Total).HasPrecision(9, 2).HasColumnName("total");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            // Relationships to Sucursal, Tarjeta, Usuario (in other DBs) are removed.
        });

        modelBuilder.Entity<Carrito>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("carrito");
            entity.HasIndex(e => e.UsuarioId, "usuario_id_UNIQUE").IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
        });

        modelBuilder.Entity<CarritoItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("carrito_items");
            entity.HasIndex(e => e.CarritoId, "fk_item_carrito_idx");
            entity.HasIndex(e => e.ProductoId, "fk_item_producto_idx");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CarritoId).HasColumnName("carrito_id");
            entity.Property(e => e.ProductoId).HasColumnName("producto_id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");

            entity.HasOne(d => d.Carrito)
                .WithMany(p => p.Items)
                .HasForeignKey(d => d.CarritoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_item_carrito");

            // Relationship to Producto (in other DB) is removed.
        });
    }
}
