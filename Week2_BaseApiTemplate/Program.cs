using Microsoft.Extensions.Options;
using Week2_BaseApiTemplate.Configurations;
using Week2_BaseApiTemplate.DTOs;
using Week2_BaseApiTemplate.Exceptions;
using Week2_BaseApiTemplate.Middlewares;
using Week2_BaseApiTemplate.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. OPTIONS PATTERN
builder.Services.Configure<WalletSettings>(builder.Configuration.GetSection("WalletSettings"));

// 2. DEPENDENCY INJECTION (DI) REGISTER
builder.Services.AddSingleton<ITransactionCounterService, TransactionCounterService>();
builder.Services.AddScoped<IWalletService, WalletService>();

var app = builder.Build();

// 3. MIDDLEWARE PIPELINE (Sıralama Hayati Önem Taşır)
app.UseMiddleware<ExceptionHandlingMiddleware>(); // En dışta hataları yakalar
app.UseMiddleware<RequestLoggingMiddleware>();   // İstek sürelerini ölçer

// 4. MINIMAL API ENDPOINTS

// Endpoint A: Options Pattern Test (Ayarları Okuma)
app.MapGet("/api/config/wallet", (IOptions<WalletSettings> options) =>
{
    var config = options.Value;
    return Results.Ok(ApiResponse<WalletSettings>.Success(config, "Cüzdan ayarları başarıyla okundu."));
});

// Endpoint B: Bakiye Sorgulama (Scoped + Singleton Testi)
app.MapGet("/api/wallet/balance", (IWalletService walletService, ITransactionCounterService counterService) =>
{
    var balance = walletService.GetBalance();
    var totalTxns = counterService.GetTotalTransactions();

    var data = new
    {
        WalletInfo = balance,
        SystemTotalTransactions = totalTxns
    };

    return Results.Ok(ApiResponse<object>.Success(data, "Bakiye bilgisi getirildi."));
});

// Endpoint C: Para Transferi (İş Mantığı + Exception Testi)
app.MapPost("/api/wallet/transfer", (
    TransferRequestDto request,
    IWalletService walletService,
    IOptions<WalletSettings> options) =>
{
    var dailyLimit = options.Value.DailyTransferLimit;
    var result = walletService.Transfer(request, dailyLimit);

    return Results.Ok(ApiResponse<TransferResponseDto>.Success(result, "Transfer işlemi başarılı."));
});

// Endpoint D: Custom Exception Test (404 Not Found)
app.MapGet("/api/test/notfound", () =>
{
    throw new NotFoundException("İstenen cüzdan hesabı sistemde bulunamadı!");
});

// Endpoint E: Unhandled Exception Test (500 Internal Server Error)
app.MapGet("/api/test/error", () =>
{
    throw new Exception("Sistemde beklenmeyen kriz oluştu!");
});

app.Run();