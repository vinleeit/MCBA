using Mcba.Models;

namespace Mcba.Services.Interfaces;

public interface IDepositService
{
    public enum DepositError
    {
        InvalidAmount,
        Unknown
    }

    public Task<DepositError?> Deposit(int accountNumber, decimal amount, string? comment);
}
