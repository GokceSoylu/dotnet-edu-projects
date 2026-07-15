using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Week1_CSharpFundamentals;

// Primary Constructor yardımıyla AppDbContext bağımlılığını enjekte ediyoruz.
public class OrderService(AppDbContext dbContext)
{
    // --- 1. DEFERRED EXECUTION & IQUERYABLE ---
    // Bu metot çağrıldığında veritabanına SQL gitmez, sadece sorgu taslağı (IQueryable) üretilir.
    public IQueryable<Order> GetCompletedOrdersQuery()
    {
        return dbContext.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.Status == "Completed");
    }

    // --- 2. IQUERYABLE VS IENUMERABLE PERFORMANS SİMÜLASYONU ---
    public void CompareBehaviors()
    {
        Console.WriteLine("\n[IQueryable Testi Başlıyor...] (Sorgular SQL sunucusunda tamamlanır)");
        // Veritabanına sadece CustomerId = 1 olan siparişleri ve LIMIT 2 filtresini SQL düzeyinde gönderir.
        var queryableResult = GetCompletedOrdersQuery()
            .Where(o => o.CustomerId == 1)
            .Take(2)
            .ToList(); // İşte SQL bu satırda tetiklenir!

        Console.WriteLine("\n[IEnumerable Testi Başlıyor...] (FİLTRE RAM'DE ÇALIŞIR!)");
        // AsEnumerable() çağırdığımız an EF Core veritabanındaki tüm 'Completed' siparişleri RAM'e çeker!
        // CustomerId = 1 ve Take(2) filtrelemeleri C# tarafında RAM üzerinde uygulanır. Ciddi bir bellek israfıdır.
        var enumerableResult = GetCompletedOrdersQuery()
            .AsEnumerable()
            .Where(o => o.CustomerId == 1) // RAM seviyesinde filtreleme
            .Take(2)
            .ToList();
    }

    // --- 3. SWITCH PATTERN MATCHING & RECORD STRUCT ---
    public PerformanceReport GeneratePerformanceReport()
    {
        var orders = dbContext.Orders.Include(o => o.OrderItems).ToList();

        int totalProcessed = orders.Count(o => o.Status is "Completed" or "Shipped");
        decimal revenue = orders
            .Where(o => o.Status != "Cancelled")
            .SelectMany(o => o.OrderItems)
            .Sum(oi => oi.Quantity * oi.Price);

        // Modern C# Pattern Matching (Tuple & Relational & Logical Patterns)
        string status = (totalProcessed, revenue) switch
        {
            (0, _) => "No Activity",
            (_, < 5000) => "Low Revenue",
            ( > 10 and <= 50, >= 5000 and < 20000) => "Optimal Operations",
            ( > 50, _) => "Outstanding Performance",
            _ => "Stable"
        };

        // Stack üzerinde yaşayan hafif bir readonly record struct dönüyoruz
        return new PerformanceReport(totalProcessed, revenue, status);
    }

    // --- 4. RECORD WITH EXPRESSION TEST ---
    public void DemonstrateRecordWith()
    {
        var details = dbContext.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .Select(o => new OrderDetailDto(
                o.OrderId,
                o.Customer.Name,
                o.OrderItems.Sum(oi => oi.Quantity * oi.Price),
                o.Status
            )).ToList();

        if (details.Any())
        {
            var original = details.First();

            // 'with' anahtar kelimesiyle orijinal nesneye dokunmadan yeni bir kopyasını türetiyoruz (Immutability)
            var updated = original with { Status = "Updated & Processed" };

            Console.WriteLine($"\nOrijinal Record DTO  : {original}");
            Console.WriteLine($"with ile Kopyalanan  : {updated}");
        }
    }
}