using WalletApi.Models;

namespace WalletApi.Repositories;

public interface IWalletRepository
{
    Task<Account> CreateAccountAsync(Account account);
    Task<Account?> GetAccountByIdAsync(Guid accountId);
    Task<IEnumerable<Account>> GetAllAccountsAsync();
    Task<bool> AccountExistsAsync(Guid accountId);
    Task<Transaction> AddTransactionAsync(Transaction transaction);
    Task<IEnumerable<Transaction>> GetStatementAsync(Guid accountId);
    Task SaveChangesAsync();
}