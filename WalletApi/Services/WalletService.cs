using WalletApi.Enums;
using WalletApi.Models;
using WalletApi.Repositories;

namespace WalletApi.Services;

public class WalletService : IWalletService
{
    private readonly IWalletRepository _repository;

    public WalletService(IWalletRepository repository)
    {
        _repository = repository;
    }

    public async Task<Account> CreateAccountAsync(string ownerName, string document)
    {
        var account = new Account
        {
            OwnerName = ownerName,
            Document = document,
            Balance = 0m
        };

        return await _repository.CreateAccountAsync(account);
    }

    public async Task<Account?> GetAccountAsync(Guid accountId)
    {
        return await _repository.GetAccountByIdAsync(accountId);
    }

    public async Task<IEnumerable<Account>> GetAllAccountsAsync()
    {
        return await _repository.GetAllAccountsAsync();
    }

    public async Task<Transaction> DepositAsync(Guid accountId, decimal amount, string? description)
    {
        if (amount <= 0)
            throw new InvalidAmountException("Deposit amount must be greater than zero.");

        var account = await _repository.GetAccountByIdAsync(accountId)
            ?? throw new AccountNotFoundException($"Account {accountId} not found.");

        account.Balance += amount;

        var transaction = new Transaction
        {
            AccountId = account.Id,
            Type = TransactionType.Deposit,
            Amount = amount,
            BalanceAfter = account.Balance,
            Description = description
        };

        await _repository.AddTransactionAsync(transaction);
        await _repository.SaveChangesAsync();

        return transaction;
    }

    public async Task<Transaction> WithdrawAsync(Guid accountId, decimal amount, string? description)
    {
        if (amount <= 0)
            throw new InvalidAmountException("Withdrawal amount must be greater than zero.");

        var account = await _repository.GetAccountByIdAsync(accountId)
            ?? throw new AccountNotFoundException($"Account {accountId} not found.");

        if (account.Balance < amount)
            throw new InsufficientFundsException("Insufficient funds for withdrawal.");

        account.Balance -= amount;

        var transaction = new Transaction
        {
            AccountId = account.Id,
            Type = TransactionType.Withdrawal,
            Amount = amount,
            BalanceAfter = account.Balance,
            Description = description
        };

        await _repository.AddTransactionAsync(transaction);
        await _repository.SaveChangesAsync();

        return transaction;
    }

    public async Task<(Transaction outgoing, Transaction incoming)> TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount, string? description)
    {
        if (amount <= 0)
            throw new InvalidAmountException("Transfer amount must be greater than zero.");

        if (fromAccountId == toAccountId)
            throw new InvalidAmountException("Cannot transfer to the same account.");

        var fromAccount = await _repository.GetAccountByIdAsync(fromAccountId)
            ?? throw new AccountNotFoundException($"Source account {fromAccountId} not found.");

        var toAccount = await _repository.GetAccountByIdAsync(toAccountId)
            ?? throw new AccountNotFoundException($"Destination account {toAccountId} not found.");

        if (fromAccount.Balance < amount)
            throw new InsufficientFundsException("Insufficient funds for transfer.");

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

        await _repository.AddTransactionAsync(outgoing);
        await _repository.AddTransactionAsync(incoming);
        await _repository.SaveChangesAsync();

        return (outgoing, incoming);
    }

    public async Task<IEnumerable<Transaction>> GetStatementAsync(Guid accountId)
    {
        var exists = await _repository.AccountExistsAsync(accountId);
        if (!exists)
            throw new AccountNotFoundException($"Account {accountId} not found.");

        return await _repository.GetStatementAsync(accountId);
    }
}