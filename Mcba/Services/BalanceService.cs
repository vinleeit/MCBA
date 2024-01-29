using Mcba.Services.Interfaces;
using McbaData;
using Microsoft.EntityFrameworkCore;

namespace Mcba.Services;

public class BalanceService(McbaContext context) : IBalanceService
{
    private readonly McbaContext _dbContext = context;

    public async Task<decimal> GetAccountBalance(int accountNumber)
    {
        // Get Money in amount
        decimal creditTransaction = await (
            from t in _dbContext.Transactions
            where
                t.AccountNumber == accountNumber
                && (
                    t.TransactionType == 'D'
                    || (t.TransactionType == 'T' && t.DestinationAccountNumber == null)
                )
            select t.Amount
        ).SumAsync();

        // Get money out amount
        decimal debitTransaction = await (
            from t in _dbContext.Transactions
            where
                t.AccountNumber == accountNumber
                && (
                    t.TransactionType == 'W'
                    || (t.TransactionType == 'T' && t.DestinationAccountNumber != null)
                    || t.TransactionType == 'S'
                    || t.TransactionType == 'B'
                )
            select t.Amount
        ).SumAsync();

        return creditTransaction - debitTransaction;
    }
}
