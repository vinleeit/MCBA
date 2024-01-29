namespace Mcba.Services.Interfaces;

public interface IFreeTransactionService
{
    public Task<bool> GetIsTransactionFree(int accountNumber);
}
