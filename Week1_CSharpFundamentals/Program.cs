using Microsoft.EntityFrameworkCore;
using Week1_CSharpFundamentals;

Console.WriteLine("=== HAFTA 1: ETİCARET ANALİZ MOTORU BAŞLADI ===\n");

// 1. Altyapının Hazırlanması
using var context = new AppDbContext();
var analysisService = new OrderAnalysisService(context);

// ==========================================
// DENEY 1: IQueryable & Deferred Execution (Ertelenmiş Çalışma)
// ==========================================
Console.WriteLine("--- 1. Deney: IQueryable Davranışı ---");

// Metodu çağırıyoruz. Metodun içinde _context.Orders sorgusu var.
var pendingOrdersQuery = analysisService.GetPendingOrShippedOrdersQuery();

Console.WriteLine("-> GetPendingOrShippedOrdersQuery metodu çağrıldı.");
Console.WriteLine("-> DİKKAT: Şu ana kadar veri tabanına henüz hiçbir SQL sorgusu GİTMEDİ!");
Console.WriteLine($"-> query değişkeninin tipi: {pendingOrdersQuery.GetType().Name}");

Console.WriteLine("\n-> Şimdi sorguyu tetikliyoruz (Belleğe çekiyoruz)...");
// Sorgu tam olarak bu satırda, döngü (foreach) veya ToList ile tetiklendiğinde SQL'e dönüşür.
var executionResult = await pendingOrdersQuery.ToListAsync().ConfigureAwait(false);
Console.WriteLine($"-> Sorgu nihayet çalıştı! Veri tabanından {executionResult.Count} adet sipariş RAM'e indi.\n");


// ==========================================
// DENEY 2: Asenkron Raporlama & IEnumerable Rapor Dönüşümü
// ==========================================
Console.WriteLine("--- 2. Deney: Uçtan Uca Asenkron Raporlama ---");
Console.WriteLine("-> Rapor hazırlanıyor (Async & ConfigureAwait)...");

var reports = await analysisService.GenerateOrderReportAsync().ConfigureAwait(false);

Console.WriteLine("\n=== AKTİF SİPARİŞ RAPORU ===");
foreach (var report in reports)
{
    // C# 12 Record yapısı ToString() metodunu otomatik ezdiği için doğrudan süslü parantez içinde basabiliyoruz.
    Console.WriteLine($"Sipariş ID: {report.OrderId} | Müşteri: {report.CustomerName} | Toplam Tutar: {report.TotalAmount:C2} | Durum: {report.OrderStatus}");
}


// ==========================================
// DENEY 3: ValueTask & Modern Pattern Matching
// ==========================================
Console.WriteLine("\n--- 3. Deney: ValueTask ve Pattern Matching Komisyon Hesabı ---");

// Örnek olması için veri tabanındaki 1 numaralı ödemeyi analiz ediyoruz
int targetPaymentId = 1;
Console.WriteLine($"-> {targetPaymentId} ID'li ödeme için komisyon simülasyonu başlatılıyor...");

// ValueTask dönen metot çağrısı
var feeAnalysis = await analysisService.CalculatePaymentFeeAsync(targetPaymentId).ConfigureAwait(false);

Console.WriteLine("\n=== ÖDEME KOMİSYON ANALİZ SONUCU ===");
Console.WriteLine($"Sipariş ID: {feeAnalysis.OrderId}");
Console.WriteLine($"Ham Tutar : {feeAnalysis.OriginalAmount:C2}");
Console.WriteLine($"Net Tutar : {feeAnalysis.FinalAmountWithFee:C2}");
Console.WriteLine($"Açıklama  : {feeAnalysis.FeeDetails}");

Console.WriteLine("\n=== ANALİZ MOTORU BAŞARIYLA TAMAMLANDI ===");