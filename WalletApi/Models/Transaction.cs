namespace WalletApi.Models;

public enum TransactionType
{
    Deposit,
    Withdrawal,
    TransferIn,
    TransferOut
}

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AccountId { get; set; }
    public Account? Account { get; set; }

    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? Description { get; set; }
    public Guid? RelatedAccountId { get; set; } // used for transfers
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
