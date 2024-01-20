using Mcba.Data;
using Mcba.Models;
using Mcba.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Mcba.Services;

public class AccountService(McbaContext context) : IAccountService
{
    private readonly McbaContext _dbContext = context;

    public async Task<List<Account>> GetAccounts(int customerNumber)
    {
        return await (
            from a in _dbContext.Accounts
            where a.CustomerID == customerNumber
            select a
        ).ToListAsync();
    }
}
