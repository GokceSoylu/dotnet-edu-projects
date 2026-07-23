namespace Week2_BaseApiTemplate.DTOs;

public record WalletBalanceDto(string AccountHolder, decimal Balance, string Currency);

public record TransferRequestDto(string ReceiverAccount, decimal Amount);

public record TransferResponseDto(string TransactionId, decimal RemainingBalance, decimal FeeCharged);