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

    public List<Order> Orders { get; set; } = [];
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

[Table("orders")]
public class Order
{
    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("customer_id")]
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    [Column("order_date")]
    public DateTime OrderDate { get; set; }

    [Column("status")]
    public string Status { get; set; } = null!;

    public List<OrderItem> OrderItems { get; set; } = [];
}

[Table("order_items")]
public class OrderItem
{
    [Column("order_item_id")]
    public int OrderItemId { get; set; }

    [Column("order_id")]
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    [Column("product_id")]
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("price")]
    public decimal Price { get; set; }
}

public record OrderDetailDto(
    int OrderId,
    string CustomerName,
    decimal TotalAmount,
    string Status
);

public readonly record struct PerformanceReport(
    int TotalProcessed,
    decimal Revenue,
    string EfficiencyStatus
);