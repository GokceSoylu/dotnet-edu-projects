using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Week1_CSharpFundamentals;

[Table("shippers")]
public class Shipper
{
    [Column("shipper_id")]
    public int ShipperId { get; set; }
    [Column("shipper_name")]
    public String ShipperName { get; set; } = null!;
}

[Table("shipments")]
public class Shipment
{
    [Column("shipment_şd")]
    public int ShipmentId { get; set; }

    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("shipper_id")]
    public int ShipperId { get; set; }
    public Shipper Shipper { get; set; }

    [Column("tracking_number")]
    public String? TrackingNumber { get; set; }

    [Column("shipped_date")]
    public DateTime? ShippedDate { get; set; }

    [Column("delivered_date")]
    public DateTime? DeliveredDate { get; set; }

    [Column("freight_cost")]
    public decimal FreightCost { get; set; }
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