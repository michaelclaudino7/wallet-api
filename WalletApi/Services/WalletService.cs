using Microsoft.EntityFrameworkCore;
using WalletApi.Data;
using WalletApi.Models;

namespace WalletApi.Services;

public class WalletService : IWalletService
{
    private readonly WalletDbContext _context;

    public WalletService(WalletDbContext context)
    {
        _context = context;
    }

    public async Task<Account> CreateAccountAsync(string ownerName, string document)
    {
        var account = new Account
        {
            OwnerName = ownerName,
            Document = document,
            Balance = 0m
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return account;
    }

    public async Task<Account?> GetAccountAsync(Guid accountId)
    {
        return await _context.Accounts.FindAsync(accountId);
    }

    public async Task<IEnumerable<Account>> GetAllAccountsAsync()
    {
        return await _context.Accounts.ToListAsync();
    }

    public async Task<Transaction> DepositAsync(Guid accountId, decimal amount, string? description)
    {
        if (amount <= 0)
            throw new InvalidAmountException("O valor do depósito deve ser maior que zero.");

        var account = await _context.Accounts.FindAsync(accountId)
            ?? throw new AccountNotFoundException($"Conta {accountId} não encontrada.");

        account.Balance += amount;

        var transaction = new Transaction
        {
            AccountId = account.Id,
            Type = TransactionType.Deposit,
            Amount = amount,
            BalanceAfter = account.Balance,
            Description = description
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }

    public async Task<Transaction> WithdrawAsync(Guid accountId, decimal amount, string? description)
    {
        if (amount <= 0)
            throw new InvalidAmountException("O valor do saque deve ser maior que zero.");

        var account = await _context.Accounts.FindAsync(accountId)
            ?? throw new AccountNotFoundException($"Conta {accountId} não encontrada.");

        if (account.Balance < amount)
            throw new InsufficientFundsException("Saldo insuficiente para realizar o saque.");

        account.Balance -= amount;

        var transaction = new Transaction
        {
            AccountId = account.Id,
            Type = TransactionType.Withdrawal,
            Amount = amount,
            BalanceAfter = account.Balance,
            Description = description
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }

    public async Task<(Transaction outgoing, Transaction incoming)> TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount, string? description)
    {
        if (amount <= 0)
            throw new InvalidAmountException("O valor da transferência deve ser maior que zero.");

        if (fromAccountId == toAccountId)
            throw new InvalidAmountException("Não é possível transferir para a mesma conta.");

        var fromAccount = await _context.Accounts.FindAsync(fromAccountId)
            ?? throw new AccountNotFoundException($"Conta de origem {fromAccountId} não encontrada.");

        var toAccount = await _context.Accounts.FindAsync(toAccountId)
            ?? throw new AccountNotFoundException($"Conta de destino {toAccountId} não encontrada.");

        if (fromAccount.Balance < amount)
            throw new InsufficientFundsException("Saldo insuficiente para realizar a transferência.");

        using var dbTransaction = await _context.Database.BeginTransactionAsync();

        try
        {
            fromAccount.Balance -= amount;
            toAccount.Balance += amount;

            var outgoing = new Transaction
            {
                AccountId = fromAccount.Id,
                Type = TransactionType.TransferOut,
                Amount = amount,
                BalanceAfter = fromAccount.Balance,
                Description = description,
                RelatedAccountId = toAccount.Id
            };

            var incoming = new Transaction
            {
                AccountId = toAccount.Id,
                Type = TransactionType.TransferIn,
                Amount = amount,
                BalanceAfter = toAccount.Balance,
                Description = description,
                RelatedAccountId = fromAccount.Id
            };

            _context.Transactions.Add(outgoing);
            _context.Transactions.Add(incoming);

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return (outgoing, incoming);
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<Transaction>> GetStatementAsync(Guid accountId)
    {
        var accountExists = await _context.Accounts.AnyAsync(a => a.Id == accountId);
        if (!accountExists)
            throw new AccountNotFoundException($"Conta {accountId} não encontrada.");

        return await _context.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}
