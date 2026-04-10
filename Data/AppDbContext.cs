using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Models;

namespace ECommerceAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product pertenece a una Category. Evita borrar categorías con productos.
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // CartItem referencia a Product. Evita borrar productos que estén en carritos.
        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Product)
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // OrderItem referencia a Product. Evita borrar productos ya comprados.
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Order tiene muchos OrderItems. Si se borra la orden, se borran sus items.
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Evita procesar el mismo pago más de una vez (idempotencia).
        modelBuilder.Entity<Order>()
            .HasIndex(o => o.MercadoPagoPaymentId)
            .IsUnique();

        // Optimiza búsquedas por estado (admin, filtros, etc).
        modelBuilder.Entity<Order>()
            .HasIndex(o => o.Status);

        // Optimiza detección de reservas vencidas.
        modelBuilder.Entity<Order>()
            .HasIndex(o => o.ReservationExpiresAt);
    }
}