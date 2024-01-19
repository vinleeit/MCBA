using Mcba.Models;

namespace Mcba.Services;

public interface IStatementService {
    public Task<(int totalPageCount, IEnumerable<Transaction> transactions)> GetPaginatedAccountTransactions(int accountNumber, (int pageNumber, int itemCount)? pagination);
}