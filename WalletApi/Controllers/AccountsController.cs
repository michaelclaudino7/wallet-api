using Microsoft.AspNetCore.Mvc;
using WalletApi.DTOs;
using WalletApi.Services;

namespace WalletApi.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly IWalletService _walletService;

    public AccountsController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    /// <summary>Cria uma nova conta na carteira digital.</summary>
    [HttpPost]
    public async Task<ActionResult<AccountResponse>> CreateAccount([FromBody] CreateAccountRequest request)
    {
        var account = await _walletService.CreateAccountAsync(request.OwnerName, request.Document);

        var response = new AccountResponse(account.Id, account.OwnerName, account.Document, account.Balance, account.CreatedAt);
        return CreatedAtAction(nameof(GetAccount), new { accountId = account.Id }, response);
    }

    /// <summary>Lista todas as contas cadastradas.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountResponse>>> GetAllAccounts()
    {
        var accounts = await _walletService.GetAllAccountsAsync();
        var response = accounts.Select(a => new AccountResponse(a.Id, a.OwnerName, a.Document, a.Balance, a.CreatedAt));
        return Ok(response);
    }

    /// <summary>Busca uma conta pelo seu identificador.</summary>
    [HttpGet("{accountId}")]
    public async Task<ActionResult<AccountResponse>> GetAccount(Guid accountId)
    {
        var account = await _walletService.GetAccountAsync(accountId);
        if (account is null)j
            return NotFound(new { message = $"Conta {accountId} não encontrada." });

        var response = new AccountResponse(account.Id, account.OwnerName, account.Document, account.Balance, account.CreatedAt);
        return Ok(response);
    }

    /// <summary>Retorna o extrato de transações de uma conta.</summary>
    [HttpGet("{accountId}/statement")]
    public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetStatement(Guid accountId)
    {
        try
        {
            var transactions = await _walletService.GetStatementAsync(accountId);
            var response = transactions.Select(t => new TransactionResponse(
                t.Id, t.AccountId, t.Type.ToString(), t.Amount, t.BalanceAfter, t.Description, t.RelatedAccountId, t.CreatedAt));

            return Ok(response);
        }
        catch (AccountNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
