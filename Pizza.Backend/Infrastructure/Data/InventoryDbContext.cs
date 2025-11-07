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
            entity.HasIndex(e => e.PedidoId, "pedido_id");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.PedidoId).HasColumnName("pedido_id");
            entity.Property(e => e.PrecioUnitario).HasPrecision(9, 2).HasColumnName("precio_unitario");

            entity.HasOne(d => d.Pedido).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("detalle_pedido_ibfk_1");
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
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado).HasMaxLength(20).HasColumnName("estado");
            entity.Property(e => e.Fecha).HasDefaultValueSql("CURRENT_TIMESTAMP").HasColumnType("datetime").HasColumnName("fecha");
            entity.Property(e => e.Total).HasPrecision(9, 2).HasColumnName("total");
            entity.Property(e => e.MetodoPago).HasMaxLength(50).HasColumnName("metodo_pago");
        });

        modelBuilder.Entity<Carrito>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("carrito");
            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<CarritoItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("carrito_items");
            entity.HasIndex(e => e.CarritoId, "fk_item_carrito_idx");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CarritoId).HasColumnName("carrito_id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");

            entity.HasOne(d => d.Carrito)
                .WithMany(p => p.Items)
                .HasForeignKey(d => d.CarritoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_item_carrito");
        });

        modelBuilder.Ignore<Oferta>();
        modelBuilder.Ignore<Producto>();
        modelBuilder.Ignore<Sucursale>();
        modelBuilder.Ignore<Usuario>();
        modelBuilder.Ignore<Menu>();
        modelBuilder.Ignore<Tarjeta>();
        modelBuilder.Ignore<Calificacione>();
    }
}
