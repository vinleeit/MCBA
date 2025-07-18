using Mcba.Services.Interfaces;
using McbaData;
using McbaData.Models;

namespace Mcba.Services;

public class DepositService(McbaContext context) : IDepositService
{
    private readonly McbaContext _dbContext = context;

    public async Task<IDepositService.DepositError?> Deposit(
        int accountNumber,
        decimal amount,
        string? comment
    )
    {
        // Deposit should be at least $0.01
        if (amount < (decimal)0.01)
        {
            return IDepositService.DepositError.InvalidAmount;
        }
        Transaction t =
            new()
            {
                AccountNumber = accountNumber,
                Amount = amount,
                Comment = comment,
                TransactionType = 'D',
                TransactionTimeUtc = DateTime.UtcNow
            };

        try
        {
            _ = await _dbContext.Transactions.AddAsync(t);
            int resultCount = await _dbContext.SaveChangesAsync();
            if (resultCount < 1)
            {
                return IDepositService.DepositError.Unknown;
            }
        }
        catch (Exception)
        {
            return IDepositService.DepositError.Unknown;
        }

        return null;
    }
}
