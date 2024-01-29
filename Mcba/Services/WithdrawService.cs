using Mcba.Services.Interfaces;
using McbaData;
using Microsoft.EntityFrameworkCore;

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

    // Fee that incurs on withdrawal outside of free transaction
    private readonly decimal _withdrawFee = (decimal)0.05;

    public async Task<Tuple<decimal, decimal>> GetTotalAmountAndMinimumAllowedBalance(
        int accountNumber,
        decimal amount
    )
    {
        // Add service charge to calculation if the transaction is not free
        if (!await _freeTransactionService.GetIsTransactionFree(accountNumber))
        {
            amount += _withdrawFee;
        }
        // Get minimum balance based on the type of account
        int minimumBalance =
            await (
                from a in _dbContext.Accounts
                where a.AccountNumber == accountNumber
                select a.AccountType
            ).FirstOrDefaultAsync() == 'S'
                ? 0
                : 300;
        return Tuple.Create<decimal, decimal>(amount, minimumBalance);
    }

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
        bool isTransactionFree = await _freeTransactionService.GetIsTransactionFree(accountNumber);
        // Get current amount
        decimal balance = await _balanceService.GetAccountBalance(accountNumber);
        // Check if balance is sufficient
        Tuple<decimal, decimal> totalAndMinimum = await GetTotalAmountAndMinimumAllowedBalance(
            accountNumber,
            amount
        );
        if (balance - totalAndMinimum.Item2 < totalAndMinimum.Item1)
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
                    Amount = _withdrawFee,
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
