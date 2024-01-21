using Mcba.Data;
using Mcba.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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
    private readonly decimal _transferFee = (decimal)0.1;

    public async Task<Tuple<decimal, decimal>> GetTotalAndMinimumBalance(int accountNumber, decimal amount)
    {
        var isFree = await _freeTransactionService.GetIsTransactionFree(accountNumber);
        if (!isFree)
        {
            amount += _transferFee;
        }
        decimal minimum = (await (from a in _dbContext.Accounts where a.AccountNumber == accountNumber select a.AccountType).FirstOrDefaultAsync()) == 'S' ? 0 : 300;
        return Tuple.Create(amount, minimum);
    }
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
        var (total, minimum) = await GetTotalAndMinimumBalance(accountNumber, amount);
        if (
            (await _balanceService.GetAccountBalance(accountNumber)) - minimum
            < total
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
