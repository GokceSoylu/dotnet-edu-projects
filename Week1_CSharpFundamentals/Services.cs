using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Week1_CSharpFundamentals;

public class OrderAnalysisService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public IQueryable<Order> GetPendingOrShippedOrdersQuery()
    {
        return _context.Orders.Where(o => o.Status == "Pending" || o.Status == "Shipped");
    }

    public async Task<List<OrderReportDto>> GenerateOrderReportAsync()
    {
        List<Order> orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .Where(o => o.OrderItems.Count > 0)
            .ToListAsync()
            .ConfigureAwait(false);

        IEnumerable<OrderReportDto> report = orders.Select(o => new OrderReportDto(
            o.OrderId,
            o.Customer.Name,
            o.OrderItems.Sum(oi => oi.Quantity * oi.Price),
            o.Status
        ));

        return report.ToList();
    }

    public string AnalyzeOrderStatusWithPattern(Order order)
    {
        return order switch
        {
            { Status: "Pending" } => "Sipariş alındı, paketleme sırası bekliyor.",
            { Status: "Shipped" } => "Sipariş kargoya verildi, yolda.",
            { Status: "Delivered" } => "Sipariş müşteriye başarıyla teslim edildi.",
            { Status: var unknownStatus } => $"Bilinmeyen sipariş durumu detected: {unknownStatus}"
        };
    }

    public async ValueTask<PaymentAnalysisResult> CalculatePaymentFeeAsync(int paymentId)
    {
        var payment = await _context.Payments
            .SingleOrDefaultAsync(p => p.PaymentId == paymentId)
            .ConfigureAwait(false);

        if (payment == null)
        {
            return new PaymentAnalysisResult(0, 0, 0, "Ödeme kaydı bulunamadı.");
        }

        await Task.Delay(50).ConfigureAwait(false);

        (decimal finalAmount, string details) = payment switch
        {
            { Status: "Completed", PaymentMethod: "Credit Card" } => (payment.Amount * 1.015m, "Kredi kartı %1.5 vade farkı uygulandı."),
            { Status: "Completed", PaymentMethod: "PayPal" } => (payment.Amount * 1.03m, "PayPal %3 komisyon uygulandı."),
            { Status: "Completed" } => (payment.Amount, "Komisyonsuz doğrudan tahsilat."),
            _ => (payment.Amount, "Başarısız ödeme, işlem ücreti hesaplanmadı.")
        };

        return new PaymentAnalysisResult(payment.OrderId, payment.Amount, finalAmount, details);
    }
}