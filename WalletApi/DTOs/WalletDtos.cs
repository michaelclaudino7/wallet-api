namespace WalletApi.DTOs;

public record CreateAccountRequest(string OwnerName, string Document);

public record AccountResponse(Guid Id, string OwnerName, string Document, decimal Balance, DateTime CreatedAt);

public record DepositRequest(decimal Amount, string? Description);

public record WithdrawRequest(decimal Amount, string? Description);

public record TransferRequest(Guid ToAccountId, decimal Amount, string? Description);

public record TransactionResponse(
    Guid Id,
    Guid AccountId,
    string Type,
    decimal Amount,
    decimal BalanceAfter,
    string? Description,
    Guid? RelatedAccountId,
    DateTime CreatedAt
);
