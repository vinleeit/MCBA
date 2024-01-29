using McbaData.Models;

namespace Mcba.Services.Interfaces;

public interface IAccountService
{
    public Task<List<Account>> GetAccounts(int customerNumber);
}
