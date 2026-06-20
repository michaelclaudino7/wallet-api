using Microsoft.EntityFrameworkCore;
using WalletApi.Data;
using WalletApi.Models;

namespace WalletApi.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly WalletDbContext _context;

    public WalletRepository(WalletDbContext context)
    {
        _context = context;
    }

    public async Task<Account> CreateAccountAsync(Account account)
    {
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<Account?> GetAccountByIdAsync(Guid accountId)
    {
        return await _context.Accounts.FindAsync(accountId);
    }

    public async Task<IEnumerable<Account>> GetAllAccountsAsync()
    {
        return await _context.Accounts.ToListAsync();
    }

    public async Task<bool> AccountExistsAsync(Guid accountId)
    {
        return await _context.Accounts.AnyAsync(a => a.Id == accountId);
    }

    public async Task<Transaction> AddTransactionAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        return transaction;
    }

    public async Task<IEnumerable<Transaction>> GetStatementAsync(Guid accountId)
    {
        return await _context.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}