using Week2_BaseApiTemplate.DTOs;
using Week2_BaseApiTemplate.Exceptions;

namespace Week2_BaseApiTemplate.Services;

// 1. Singleton Sayaç Servisi (Uygulama açık kaldığı sürece toplam işlem sayısını tutar)
public interface ITransactionCounterService
{
    int IncrementAndGet();
    int GetTotalTransactions();
}

public class TransactionCounterService : ITransactionCounterService
{
    private int _count = 0;

    public int IncrementAndGet() => Interlocked.Increment(ref _count);
    public int GetTotalTransactions() => _count;
}

// 2. Scoped Cüzdan Servisi (Her HTTP isteğinde iş mantığını yürütür)
public interface IWalletService
{
    WalletBalanceDto GetBalance();
    TransferResponseDto Transfer(TransferRequestDto request, decimal dailyLimit);
}

public class WalletService : IWalletService
{
    // Simüle edilmiş bakiye
    private static decimal _currentBalance = 10000.00m;
    private readonly ITransactionCounterService _counterService;

    public WalletService(ITransactionCounterService counterService)
    {
        _counterService = counterService;
    }

    public WalletBalanceDto GetBalance()
    {
        return new WalletBalanceDto("Gökçe Soylu", _currentBalance, "TRY");
    }

    public TransferResponseDto Transfer(TransferRequestDto request, decimal dailyLimit)
    {
        if (request.Amount > dailyLimit)
        {
            throw new InsufficientBalanceException($"Transfer tutarı ({request.Amount} TRY) günlük limiti ({dailyLimit} TRY) aşıyor!");
        }

        if (request.Amount > _currentBalance)
        {
            throw new InsufficientBalanceException($"Yetersiz bakiye! Mevcut bakiye: {_currentBalance} TRY");
        }

        // Bakiye düşme işlemi ve sayaç arttırma
        _currentBalance -= request.Amount;
        _counterService.IncrementAndGet();

        string transactionId = $"TXN-{Guid.NewGuid().ToString()[..8].ToUpper()}";

        return new TransferResponseDto(transactionId, _currentBalance, 0.00m);
    }
}