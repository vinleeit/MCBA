using Mcba.Services.Interfaces;
using McbaData;
using Microsoft.EntityFrameworkCore;

namespace Mcba.Services;

public class FreeTransactionService(McbaContext context) : IFreeTransactionService
{
    private readonly McbaContext _dbContext = context;

    public async Task<bool> GetIsTransactionFree(int accountNumber)
    {
        int numberOfTransaction = await (
            from t in _dbContext.Transactions
            where
                t.AccountNumber == accountNumber
                && (
                    t.TransactionType == 'W'
                    || (t.TransactionType == 'T' && t.DestinationAccountNumber != null)
                )
            select t
        ).CountAsync();

        return numberOfTransaction < 2;
    }
}
