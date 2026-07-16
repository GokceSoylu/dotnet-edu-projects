using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Week1_CSharpFundamentals;

[Table("customers")]
public class Customer
{
    [Column("customer_id")]
    public int CustomerId { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("email")]
    public string Email { get; set; } = null!;
}

[Table("products")]
public class Product
{
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("price")]
    public decimal Price { get; set; }

    [Column("stock")]
    public int Stock { get; set; }
}

[Table("shippers")]
public class Shipper
{
    [Column("shipper_id")]
    public int ShipperId { get; set; }

    [Column("shipper_name")]
    public string ShipperName { get; set; } = null!;
}

[Table("shipments")]
public class Shipment
{
    [Column("shipment_id")]
    public int ShipmentId { get; set; }

    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("shipper_id")]
    public int ShipperId { get; set; }
    public Shipper Shipper { get; set; } = null!;

    [Column("tracking_number")]
    public string? TrackingNumber { get; set; }

    [Column("shipped_date")]
    public DateTime? ShippedDate { get; set; }

    [Column("delivered_date")]
    public DateTime? DeliveredDate { get; set; }

    // PostgreSQL varchar tipini karşılamak için string yapıldı
    [Column("freight_cost")]
    public string? FreightCost { get; set; }
}

[Table("orders")]
public class Order
{
    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("order_date")]
    public DateTime OrderDate { get; set; }

    [Column("status")]
    public string Status { get; set; } = null!;

    public List<Shipment> Shipments { get; set; } = [];
}

public record ShipmentDetailDto(
    int ShipmentId,
    string ShipperName,
    string TrackingNumber,
    decimal FreightCost,
    string DeliveryStatus
);

public readonly record struct CarrierPerformanceReport(
    int TotalShipments,
    decimal TotalFreightCost,
    string EfficiencyRating
);