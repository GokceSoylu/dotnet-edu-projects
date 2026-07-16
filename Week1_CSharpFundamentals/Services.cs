using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Week1_CSharpFundamentals;

public class ShippingService(AppDbContext dbContext)
{
    public IQueryable<Shipment> GetPendingShipmentsQuery()
    {
        return dbContext.Shipments
            .Include(s => s.Shipper)
            .Where(s => s.ShippedDate == null);
    }

    public void CompareShippingBehaviors()
    {
        Console.WriteLine("\n[IQueryable Sorgu Testi] (Filtreleme SQL Sunucusunda Yapılır)");
        var queryableResult = GetPendingShipmentsQuery()
            .Where(s => s.ShipperId == 1)
            .Take(2)
            .ToList();

        Console.WriteLine("\n[IEnumerable Sorgu Testi] (Filtreleme RAM'de Yapılır!)");
        var enumerableResult = GetPendingShipmentsQuery()
            .AsEnumerable()
            .Where(s => s.ShipperId == 1)
            .Take(2)
            .ToList();
    }

    public CarrierPerformanceReport AnalyzeCarrierPerformance(int shipperId)
    {
        var shipments = dbContext.Shipments
            .Where(s => s.ShipperId == shipperId)
            .ToList();

        int totalShipments = shipments.Count;

        // String olan FreightCost alanını güvenli bir şekilde decimal'a çevirip topluyoruz
        decimal totalFreightCost = shipments
            .Sum(s => decimal.TryParse(s.FreightCost, out var val) ? val : 0m);

        double averageDeliveryDays = shipments
            .Where(s => s.ShippedDate.HasValue && s.DeliveredDate.HasValue)
            .Select(s => (s.DeliveredDate!.Value - s.ShippedDate!.Value).TotalDays)
            .DefaultIfEmpty(0)
            .Average();

        string efficiencyRating = (totalShipments, averageDeliveryDays, totalFreightCost) switch
        {
            (0, _, _) => "No Activity",
            ( > 50, > 5.0, _) => "Critical Delay: High Volume But Slow",
            (_, <= 2.0, < 10000) => "Excellent: Fast and Cost-Effective",
            (_, > 2.0 and <= 5.0, >= 10000) => "Standard: High Cost",
            _ => "Stable"
        };

        return new CarrierPerformanceReport(totalShipments, totalFreightCost, efficiencyRating);
    }

    public void DemonstrateDtoCopy()
    {
        var shipmentDto = dbContext.Shipments
            .Include(s => s.Shipper)
            .AsEnumerable() // Tip dönüştürme (TryParse) işlemini RAM tarafında güvenle yapabilmek için
            .Select(s => new ShipmentDetailDto(
                s.ShipmentId,
                s.Shipper.ShipperName,
                s.TrackingNumber ?? "PENDING",
                decimal.TryParse(s.FreightCost, out var cost) ? cost : 0m,
                s.ShippedDate == null ? "Preparing" : "In Transit"
            ))
            .FirstOrDefault();

        if (shipmentDto is not null)
        {
            var updatedDto = shipmentDto with
            {
                TrackingNumber = "TRK-999888777",
                DeliveryStatus = "Shipped"
            };

            Console.WriteLine($"Orijinal Record: {shipmentDto}");
            Console.WriteLine($"Kopyalanan Record: {updatedDto}");
        }
    }
}