using WalletApi.Models;

namespace WalletApi.Services;

public interface IWalletService
{
    Task<Account> CreateAccountAsync(string ownerName, string document);
    Task<Account?> GetAccountAsync(Guid accountId);
    Task<IEnumerable<Account>> GetAllAccountsAsync();
    Task<Transaction> DepositAsync(Guid accountId, decimal amount, string? description);
    Task<Transaction> WithdrawAsync(Guid accountId, decimal amount, string? description);
    Task<(Transaction outgoing, Transaction incoming)> TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount, string? description);
    Task<IEnumerable<Transaction>> GetStatementAsync(Guid accountId);
}
