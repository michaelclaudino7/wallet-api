using Microsoft.AspNetCore.Mvc;
using WalletApi.DTOs;
using WalletApi.Services;

namespace WalletApi.Controllers;

[ApiController]
[Route("api/accounts/{accountId}")]
public class TransactionsController : ControllerBase
{
    private readonly IWalletService _walletService;

    public TransactionsController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    /// <summary>Realiza um depósito na conta informada.</summary>
    [HttpPost("deposit")]
    public async Task<ActionResult<TransactionResponse>> Deposit(Guid accountId, [FromBody] DepositRequest request)
    {
        try
        {
            var transaction = await _walletService.DepositAsync(accountId, request.Amount, request.Description);
            return Ok(MapToResponse(transaction));
        }
        catch (AccountNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidAmountException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Realiza um saque na conta informada.</summary>
    [HttpPost("withdraw")]
    public async Task<ActionResult<TransactionResponse>> Withdraw(Guid accountId, [FromBody] WithdrawRequest request)
    {
        try
        {
            var transaction = await _walletService.WithdrawAsync(accountId, request.Amount, request.Description);
            return Ok(MapToResponse(transaction));
        }
        catch (AccountNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InsufficientFundsException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidAmountException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Transfere um valor da conta informada para outra conta.</summary>
    [HttpPost("transfer")]
    public async Task<ActionResult<TransactionResponse>> Transfer(Guid accountId, [FromBody] TransferRequest request)
    {
        try
        {
            var (outgoing, _) = await _walletService.TransferAsync(accountId, request.ToAccountId, request.Amount, request.Description);
            return Ok(MapToResponse(outgoing));
        }
        catch (AccountNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InsufficientFundsException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidAmountException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private static TransactionResponse MapToResponse(Models.Transaction t) =>
        new(t.Id, t.AccountId, t.Type.ToString(), t.Amount, t.BalanceAfter, t.Description, t.RelatedAccountId, t.CreatedAt);
}
