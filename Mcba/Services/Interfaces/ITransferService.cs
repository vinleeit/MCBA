namespace Mcba.Services.Interfaces;

public interface ITransferService
{
    public enum TransferError
    {
        InvalidAmount,
        DestinationNotFound,
        NotEnoughBalance,
        Unknown
    }

    public Task<TransferError?> Transfer(
        int accountNumber,
        int destinationAccountNumber,
        decimal amount,
        string? comment
    );
}
