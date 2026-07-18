using Microsoft.EntityFrameworkCore;

namespace Week1_CSharpFundamentals;

using Week1_CSharpFundamentals;
public class OrderAnalysisService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    // 1. IQueryable ve Deferred Execution Deneyimi (Filtreleme)
    public IQueryable<Order> GetPendingOrShippedOrdersQuery()
    {
        // SQL sorgusu henüz veri tabanına GİTMEDİ. Sadece sorgu planı hazırlandı (Deferred Execution).
        var query = _context.Orders
            .Where(o => o.Status == "Pending" || o.Status == "Shipped");

        return query;
    }

    // 2. Asenkron Veri Çekme ve IEnumerable ile Bellek İçi Raporlama
    public async Task<List<OrderReportDto>> GenerateOrderReportAsync()
    {
        // IQueryable olarak sorguyu hazırlıyoruz (SQL tarafında çalışacak kısım)
        IQueryable<Order> orderQuery = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .Where(o => o.OrderItems.Count > 0);

        // Sorgu tam bu satırda, ToListAsync() tetiklendiğinde asenkron olarak SQL'e gider.
        // ConfigureAwait(false) kullanarak bu thread'in UI context'ine geri dönme zorunluluğunu kaldırıyoruz.
        List<Order> orders = await orderQuery
            .ToListAsync()
            .ConfigureAwait(false);

        // Veri belleğe (In-Memory) indi. Artık IEnumerable dünyasındayız.
        // C# 12 Primary Constructor özelliğine sahip Record nesnelerimizi üretiyoruz.
        IEnumerable<OrderReportDto> report = orders.Select(o => new OrderReportDto(
            o.OrderId,
            o.Customer.Name,
            o.OrderItems.Sum(oi => oi.Quantity * oi.Price),
            o.Status
        ));

        return report.ToList();
    }

    // 3. Modern Pattern Matching Yapısı
    public string AnalyzeOrderStatusWithPattern(Order order)
    {
        // C# 12/13 Switch Expression ve Property Pattern bir arada kullanımı
        return order switch
        {
            { Status: "Pending" } => "Sipariş alındı, paketleme sırası bekliyor.",
            { Status: "Shipped" } => "Sipariş kargoya verildi, yolda.",
            { Status: "Delivered" } => "Sipariş müşteriye başarıyla teslim edildi.",
            { Status: var unknownStatus } => $"Bilinmeyen sipariş durumu detected: {unknownStatus}"
        };
    }

    // 4. ValueTask ve Gelişmiş Ödeme/Komisyon Hesaplama Simülasyonu
    public async ValueTask<PaymentAnalysisResult> CalculatePaymentFeeAsync(int paymentId)
    {
        // SingleOrDefaultAsync veri tabanından tek bir kayıt çeker.
        var payment = await _context.Payments
            .SingleOrDefaultAsync(p => p.PaymentId == paymentId)
            .ConfigureAwait(false);

        if (payment == null)
        {
            return new PaymentAnalysisResult(0, 0, 0, "Ödeme kaydı bulunamadı.");
        }

        // Simüle edilmiş kısa bir asenkron gecikme (Örn: Banka API kontrolü)
        await Task.Delay(50).ConfigureAwait(false);

        // C# Modern Pattern Matching ile komisyon hesaplama kuralları
        (decimal finalAmount, string details) = payment switch
        {
            // Ödeme başarılı ve Credit Card ise %1.5 komisyon
            { Status: "Completed", PaymentMethod: "Credit Card" }
                => (payment.Amount * 1.015m, "Kredi kartı %1.5 vade farkı uygulandı."),

            // Ödeme başarılı ve PayPal ise %3 sabit komisyon
            { Status: "Completed", PaymentMethod: "PayPal" }
                => (payment.Amount * 1.03m, "PayPal %3 komisyon uygulandı."),

            // Ödeme başarılı ama diğer yöntemler ise komisyonsuz
            { Status: "Completed" }
                => (payment.Amount, "Komisyonsuz doğrudan tahsilat."),

            // Ödeme başarısız veya iptal ise işlem ücreti yok
            _ => (payment.Amount, "Başarısız ödeme, işlem ücreti hesaplanmadı.")
        };

        return new PaymentAnalysisResult(
            payment.OrderId,
            payment.Amount,
            finalAmount,
            details
        );
    }
}