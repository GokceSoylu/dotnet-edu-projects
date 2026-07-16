using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Week1_CSharpFundamentals;

var connectionString = "Host=localhost;Port=5433;Database=thesisdb;Username=admin;Password=admin123";
var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseNpgsql(connectionString)
              .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);

using var dbContext = new AppDbContext(optionsBuilder.Options);

Console.WriteLine("Veritabanı kontrol ediliyor, test verileri hazırlanıyor...");
dbContext.Database.EnsureCreated();

SeedDatabase(dbContext);

var shippingService = new ShippingService(dbContext);

Console.WriteLine("\n=== DENEY 1: DEFERRED EXECUTION & IQUERYABLE ===");
var query = shippingService.GetPendingShipmentsQuery();
Console.WriteLine(">> Sorgu taslağı (IQueryable) tanımlandı. SQL tetiklenmedi!");

Console.WriteLine("\n>> Şimdi .ToList() çağrılıyor. SQL tam olarak bu satırda çalışacak:");
var pendingShipments = query.ToList();

Console.WriteLine("\n=== DENEY 2: IQUERYABLE VS IENUMERABLE PERFORMANS ===");
shippingService.CompareShippingBehaviors();

Console.WriteLine("\n=== DENEY 3: MODERN SWITCH PATTERN MATCHING & RECORD STRUCT ===");
var report = shippingService.AnalyzeCarrierPerformance(1);
Console.WriteLine($"\nOluşturulan Rapor (Stack - Record Struct): {report}");

Console.WriteLine("\n=== DENEY 4: RECORD IMMUTABILITY VE WITH İFADESİ ===");
shippingService.DemonstrateDtoCopy();


static void SeedDatabase(AppDbContext db)
{
    if (db.Shippers.Any()) return;

    var shipper = new Shipper { ShipperName = "Hızlı Kargo A.Ş." };
    db.Shippers.Add(shipper);
    db.SaveChanges();

    var order1 = new Order { OrderDate = DateTime.UtcNow, Status = "Processing" };
    var order2 = new Order { OrderDate = DateTime.UtcNow.AddDays(-3), Status = "Completed" };
    db.Orders.AddRange(order1, order2);
    db.SaveChanges();

    db.Shipments.AddRange(
        new Shipment
        {
            OrderId = order1.OrderId,
            ShipperId = shipper.ShipperId,
            TrackingNumber = null,
            ShippedDate = null,
            DeliveredDate = null,
            FreightCost = "150.00" // SQL varchar uyumu için decimal yerine string verildi
        },
        new Shipment
        {
            OrderId = order2.OrderId,
            ShipperId = shipper.ShipperId,
            TrackingNumber = "TRK-123456",
            ShippedDate = DateTime.UtcNow.AddDays(-3),
            DeliveredDate = DateTime.UtcNow.AddDays(-1),
            FreightCost = "450.00" // SQL varchar uyumu için decimal yerine string verildi
        }
    );
    db.SaveChanges();

    Console.WriteLine(">>> Örnek kargo ve sipariş verileri başarıyla yüklendi! <<<\n");
}