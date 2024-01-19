namespace Mcba.Services.Interfaces;

public interface IBalanceService
{
    public Task<decimal> GetAccountBalance(int accountNumber);
}
