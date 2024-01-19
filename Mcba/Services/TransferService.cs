using Mcba.Data;
using Mcba.Services.Interfaces;

namespace Mcba.Services;

public class TransferService(
    McbaContext context,
    IBalanceService balanceService,
    IFreeTransactionService freeTransactionService
) : ITransferService
{
    private readonly McbaContext _dbContext = context;
    private readonly IBalanceService _balanceService = balanceService;
    private readonly IFreeTransactionService _freeTransactionService = freeTransactionService;

    public async Task<ITransferService.TransferError?> Transfer(
        int accountNumber,
        int destinationAccountNumber,
        decimal amount,
        string? comment
    )
    {
        // Check if it match minimum amount
        if (amount < (decimal)0.01)
        {
            return ITransferService.TransferError.InvalidAmount;
        }
        // Get if transaction is free
        bool isTransactionFree = await _freeTransactionService.GetIsTransactionFree(accountNumber);
        //0.10
        // Check if balance is enough
        if (
            await _balanceService.GetAccountBalance(accountNumber)
            < (amount + (decimal)(isTransactionFree ? 0 : 0.1))
        )
        {
            return ITransferService.TransferError.NotEnoughBalance;
        }
        // Check if destinatin account is valid
        int count = (
            from t in _dbContext.Accounts
            where t.AccountNumber == destinationAccountNumber
            select t
        ).Count();
        if (count < 1)
        {
            return ITransferService.TransferError.DestinationNotFound;
        }

        // Process transaction

        using var dbTransaction = await _dbContext.Database.BeginTransactionAsync();
        var timeUtc = DateTime.UtcNow;
        // Add transaction for this account
        _dbContext.Transactions.Add(
            new()
            {
                AccountNumber = accountNumber,
                Amount = amount,
                Comment = comment,
                TransactionType = 'T',
                DestinationAccountNumber = destinationAccountNumber,
                TransactionTimeUtc = timeUtc,
            }
        );
        int resultCount = await _dbContext.SaveChangesAsync();
        if (resultCount < 1)
        {
            await dbTransaction.RollbackAsync();
        }
        // Add service charge
        _dbContext.Transactions.Add(
            new()
            {
                AccountNumber = accountNumber,
                Amount = (decimal)0.1,
                TransactionType = 'S',
                TransactionTimeUtc = timeUtc,
            }
        );
        resultCount = await _dbContext.SaveChangesAsync();
        if (resultCount < 1)
        {
            await dbTransaction.RollbackAsync();
        }

        // Add transaction for destination account
        _dbContext.Transactions.Add(
            new()
            {
                AccountNumber = destinationAccountNumber,
                Amount = amount,
                Comment = comment,
                TransactionType = 'T',
                TransactionTimeUtc = timeUtc,
            }
        );
        resultCount = await _dbContext.SaveChangesAsync();
        if (resultCount < 1)
        {
            await dbTransaction.RollbackAsync();
        }

        await dbTransaction.CommitAsync();

        return null;
    }
}
