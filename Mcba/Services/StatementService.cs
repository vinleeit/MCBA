using Mcba.Services.Interfaces;
using McbaData;
using McbaData.Models;
using Microsoft.EntityFrameworkCore;

namespace Mcba.Services;

public class StatementService(McbaContext context, IBalanceService balanceService)
    : IStatementService
{
    private readonly McbaContext _dbContext = context;
    private readonly IBalanceService _balanceService = balanceService;

    /// <summary>
    /// Get the transactions of an account with pagination.
    /// All transactions retrieved if pagination not provided.
    /// </summary>
    /// <param name="accountNumber">the account that associates the transactions</param>
    /// <param name="pagination">the intended start item page and the number of items</param>
    /// <returns>the total page count and result transactions</returns>
    public async Task<(
        int totalPageCount,
        IEnumerable<Transaction> transactions
    )> GetPaginatedAccountTransactions(
        int accountNumber,
        (int pageNumber, int itemCount)? pagination = null
    )
    {
        var allTransactionsQuery = _dbContext
            .Transactions.Where(b => b.AccountNumber == accountNumber)
            .OrderByDescending(b => b.TransactionTimeUtc)
            .ThenByDescending(b => b.TransactionID);

        if (pagination.HasValue)
        {
            pagination = (
                (pagination.Value.pageNumber < 1) ? 1 : pagination.Value.pageNumber,
                (pagination.Value.itemCount < 1) ? 1 : pagination.Value.itemCount
            );
            var totalPageCount = (int)
                Math.Ceiling(
                    (await allTransactionsQuery.CountAsync()) / (decimal)pagination.Value.itemCount
                );
            return (
                totalPageCount,
                await allTransactionsQuery
                    .Skip((pagination.Value.pageNumber - 1) * pagination.Value.itemCount)
                    .Take(pagination.Value.itemCount)
                    .ToListAsync()
            );
        }
        return (1, await allTransactionsQuery.ToListAsync());
    }
}

