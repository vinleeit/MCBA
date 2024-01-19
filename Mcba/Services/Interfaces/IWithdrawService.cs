namespace Mcba.Services.Interfaces;

public interface IWithdrawService
{
    public enum WithdrawError
    {
        NotEnoughBalance,
        InvalidAmount,
        Unknown,
    }

    public Task<WithdrawError?> Withdraw(int accountNumber, decimal amount, string? comment);
}
