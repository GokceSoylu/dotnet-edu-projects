using Microsoft.EntityFrameworkCore;

namespace Week1_CSharpFundamentals;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<Shipper> Shippers => Set<Shipper>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>().HasKey(o => o.OrderId);
        modelBuilder.Entity<Shipment>().HasKey(s => s.ShipmentId);
        modelBuilder.Entity<Shipper>().HasKey(sh => sh.ShipperId);

        modelBuilder.Entity<Shipment>()
            .HasOne<Order>()
            .WithMany(o => o.Shipments)
            .HasForeignKey(s => s.OrderId);

        modelBuilder.Entity<Shipment>()
            .HasOne(s => s.Shipper)
            .WithMany()
            .HasForeignKey(s => s.ShipperId);
    }
}