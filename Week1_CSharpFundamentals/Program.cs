using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Week1_CSharpFundamentals;

// 1. Docker üzerindeki tez container'ı port ayarı (5433)
var connectionString = "Host=localhost;Port=5433;Database=thesisdb;Username=admin;Password=admin123";
var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseNpgsql(connectionString)
              // Arka planda üretilen SQL'leri konsolda görmek için loglamayı açıyoruz!
              .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);

using var dbContext = new AppDbContext(optionsBuilder.Options);

Console.WriteLine("Veritabanı kontrol ediliyor, test verileri hazırlanıyor...");
dbContext.Database.EnsureCreated();

// DB sıfırsa test verilerini yükleyelim
SeedDatabase(dbContext);

var orderService = new OrderService(dbContext);

Console.WriteLine("\n=== DENEY 1: DEFERRED EXECUTION & IQUERYABLE ===");
var query = orderService.GetCompletedOrdersQuery();
Console.WriteLine(">> Sorgu tanımlandı ama henüz SQL tetiklenmedi! Konsolda 'SELECT' görmemelisiniz.");

Console.WriteLine("\n>> Şimdi .ToList() çağrılarak veri çekiliyor. Dikkat edin SQL şimdi çalışacak:");
var completedOrders = query.ToList();

Console.WriteLine("\n=== DENEY 2: IQUERYABLE VS IENUMERABLE PERFORMANS ===");
orderService.CompareBehaviors();

Console.WriteLine("\n=== DENEY 3: RECORD IMMUTABILITY VE WITH İFADESİ ===");
orderService.DemonstrateRecordWith();

Console.WriteLine("\n=== DENEY 4: SWITCH PATTERN MATCHING & RECORD STRUCT ===");
var report = orderService.GeneratePerformanceReport();
Console.WriteLine($"Oluşturulan Rapor Yapısı: {report}");


// Yardımcı Örnek Veri Metodu
static void SeedDatabase(AppDbContext db)
{
    if (db.Customers.Any()) return; // Zaten veri varsa ekleme yapma

    var customer = new Customer { Name = "Gökçe Soylu", Email = "gokce@edu.com" };
    var p1 = new Product { Name = "Developer Mechanical Keyboard", Price = 1500, Stock = 100 };
    var p2 = new Product { Name = "Ergonomic Office Chair", Price = 4500, Stock = 50 };

    db.Customers.Add(customer);
    db.Products.AddRange(p1, p2);
    db.SaveChanges();

    var order = new Order { Customer = customer, OrderDate = DateTime.UtcNow, Status = "Completed" };
    db.Orders.Add(order);
    db.SaveChanges();

    db.OrderItems.AddRange(
        new OrderItem { Order = order, Product = p1, Quantity = 1, Price = p1.Price },
        new OrderItem { Order = order, Product = p2, Quantity = 1, Price = p2.Price }
    );
    db.SaveChanges();

    Console.WriteLine(">>> Örnek veriler başarıyla veritabanına yazıldı! <<<\n");
}