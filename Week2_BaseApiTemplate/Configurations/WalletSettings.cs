namespace Week2_BaseApiTemplate.Configurations;

public class WalletSettings
{
    public decimal DailyTransferLimit { get; set; }
    public decimal TransactionFeeRate { get; set; }
    public string Currency { get; set; } = "TRY";
}