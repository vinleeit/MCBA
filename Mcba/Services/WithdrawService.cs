using Mcba.Data;
using Mcba.Services.Interfaces;

namespace Mcba.Services;

public class WithdrawService(
    McbaContext context,
    IBalanceService balanceService,
    IFreeTransactionService freeTransactionService
) : IWithdrawService
{
    private readonly McbaContext _dbContext = context;
    private readonly IBalanceService _balanceService = balanceService;
    private readonly IFreeTransactionService _freeTransactionService = freeTransactionService;
    private readonly decimal _withdrawFee = (decimal)0.05;

    public async Task<IWithdrawService.WithdrawError?> Withdraw(
        int accountNumber,
        decimal amount,
        string? comment
    )
    {
        if (amount < (decimal)0.01)
        {
            return IWithdrawService.WithdrawError.InvalidAmount;
        }
        // Checks if transaction is free
        var isTransactionFree = await _freeTransactionService.GetIsTransactionFree(accountNumber);
        // Get current amount
        var balance = await _balanceService.GetAccountBalance(accountNumber);
        // Check if balance is sufficient
        // TODO: Add minimum balance check
        if (balance < (amount + (decimal)(isTransactionFree ? 0 : 0.05)))
        {
            return IWithdrawService.WithdrawError.NotEnoughBalance;
        }

        // Start transaction
        using var dbTransaction = _dbContext.Database.BeginTransaction();
        var timeUtc = DateTime.UtcNow;
        _ = _dbContext.Transactions.Add(
            new()
            {
                Amount = amount,
                TransactionType = 'W',
                Comment = comment,
                AccountNumber = accountNumber,
                TransactionTimeUtc = timeUtc
            }
        );
        int changedRows = await _dbContext.SaveChangesAsync();
        if (changedRows < 1)
        {
            await dbTransaction.RollbackAsync();
            return IWithdrawService.WithdrawError.Unknown;
        }
        if (!isTransactionFree)
        {
            // Add service charge
            _ = _dbContext.Transactions.Add(
                new()
                {
                    Amount = (decimal)0.05,
                    AccountNumber = accountNumber,
                    TransactionType = 'S',
                    TransactionTimeUtc = timeUtc,
                }
            );
            changedRows = await _dbContext.SaveChangesAsync();
            if (changedRows < 1)
            {
                await dbTransaction.RollbackAsync();
                return IWithdrawService.WithdrawError.Unknown;
            }
        }
        await dbTransaction.CommitAsync();
        return null;
    }
}
