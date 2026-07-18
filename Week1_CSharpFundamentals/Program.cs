using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Week1_CSharpFundamentals;

Console.WriteLine("=== HAFTA 1: ETİCARET ANALİZ MOTORU BAŞLADI ===\n");

using var context = new AppDbContext();
var analysisService = new OrderAnalysisService(context);

// Deney 1: Deferred Execution
Console.WriteLine("--- 1. Deney: IQueryable Davranışı ---");
var pendingOrdersQuery = analysisService.GetPendingOrShippedOrdersQuery();
Console.WriteLine("-> Sorgu planı hazırlandı, henüz SQL'e gitmedi.");

var executionResult = await pendingOrdersQuery.ToListAsync().ConfigureAwait(false);
Console.WriteLine($"-> Sorgu tetiklendi! {executionResult.Count} adet sipariş veri tabanından çekildi.\n");

// Deney 2: Asenkron Raporlama
Console.WriteLine("--- 2. Deney: Uçtan Uca Asenkron Raporlama ---");
var reports = await analysisService.GenerateOrderReportAsync().ConfigureAwait(false);

Console.WriteLine("\n=== AKTİF SİPARİŞ RAPORU ===");
foreach (var report in reports)
{
    Console.WriteLine($"Sipariş ID: {report.OrderId} | Müşteri: {report.CustomerName} | Tutar: {report.TotalAmount:C2} | Durum: {report.OrderStatus}");
}

// Deney 3: Pattern Matching
Console.WriteLine("\n--- 3. Deney: ValueTask ve Pattern Matching Komisyon Hesabı ---");
int targetPaymentId = 1;
var feeAnalysis = await analysisService.CalculatePaymentFeeAsync(targetPaymentId).ConfigureAwait(false);

Console.WriteLine("\n=== ÖDEME KOMİSYON ANALİZ SONUCU ===");
Console.WriteLine($"Sipariş ID: {feeAnalysis.OrderId} | Ham: {feeAnalysis.OriginalAmount:C2} | Net: {feeAnalysis.FinalAmountWithFee:C2} | Not: {feeAnalysis.FeeDetails}");

Console.WriteLine("\n=== ANALİZ MOTORU BAŞARIYLA TAMAMLANDI ===");