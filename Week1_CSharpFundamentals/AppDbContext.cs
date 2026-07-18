using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace Week1_CSharpFundamentals;

using Week1_CSharpFundamentals;
public class AppDbContext : DbContext
{
    // Veri tabanı tablolarını temsil eden Dbset tanımları
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;

    // 1. OnConfiguring: Bağlantı dizesinin (Connection String) tanımlandığı yer
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Docker üzerindeki PostgreSQL (tez) konteynerine bağlanıyoruz
            optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=tez;Username=admin;Password=admin123");
        }
    }
    // 2. OnModelCreating: Tablo ilişkilerinin ve şema kurallarının netleştirildiği yer
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //order_item tablosunun biirncil anahtarını (Primary Key) tanımlıyoruz
        modelBuilder.Entity<OrderItem>().HasKey(oi => oi.OrderItemId);

        // Bire-Çok (One-to-Many) İlişki Yapılandırmaları

        // Customer (1) <---> (Çok) Order
        modelBuilder.Entity<Order>()
        .HasOne(o => o.Customer)
        .WithMany(c => c.Orders)
        .HasForeignKey(o => o.CustomerID);

        // Order (1) <---> (Çok) OrderItem
        modelBuilder.Entity<OrderItems>()
        .HasOne(oi => oi.Order)
        .WithMany(o => o.OrderItems)
        .HasForeignKey(oi => oi.OrderId);

        // Product (1) <---> (Çok) OrderItem
        modelBuilder.Entity<OrderItem>()
        .HasOne(oi => oi.Product)
        .WithMany()// Product içinden tersine bir listeye şu an ihtiyacımız yok, boş bıraktık
        .HasForeignKey(oi => oi.ProductID);

        // Order (1) <---> (Çok) Payment
        modelBuilder.Entity<Payment>()
        .HasOne(p => p.Order)
        .WithMany(o => o.Payments)
        .HasForeignKey(p => p.OrderID);
    }
}