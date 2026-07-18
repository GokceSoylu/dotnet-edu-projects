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

    [Column("gender")]
    public string? Gender { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
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

    [Column("order_date")]
    public DateTime OrderDate { get; set; }

    [Column("status")]
    public string Status { get; set; } = null!;

    // Navigation Properties
    public Customer Customer { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

[Table("order_items")]
public class OrderItem
{
    [Column("order_item_id")]
    public int OrderItemId { get; set; }

    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("price")]
    public decimal Price { get; set; }

    // Navigation Properties
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

[Table("payments")]
public class Payment
{
    [Column("payment_id")]
    public int PaymentId { get; set; }

    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("payment_method")]
    public string PaymentMethod { get; set; } = null!;

    [Column("status")]
    public string Status { get; set; } = null!;

    // Navigation Property
    public Order Order { get; set; } = null!;
}

// C# 12/13 Record Yapıları
public record OrderReportDto(int OrderId, string CustomerName, decimal TotalAmount, string OrderStatus);
public readonly record struct FinancialMetrics(decimal TotalRevenue, decimal TotalTax, decimal NetProfit);
public record PaymentAnalysisResult(int OrderId, decimal OriginalAmount, decimal FinalAmountWithFee, string FeeDetails);