namespace McbaData.Tests.ModelTests;

using System.ComponentModel.DataAnnotations;
using McbaData.Models;
using Xunit;

public class TransactionTests
{
    [Theory]
    [InlineData('D', 1000, 9999, 0.01, "Hello")]
    [InlineData('W', 9999, 1000, 0.1, "Hello")]
    [InlineData('T', 1000, 9999, 1, "Hello")]
    [InlineData('S', 1000, null, 10, null)]
    [InlineData('B', 1000, 9999, 100, "123456789012345678901234567890")]
    public void CreateModel_ValidParameter(char transactionType, int accountNumber, int? destinationAccountNumber, decimal amount, string? comment)
    {
        var model = new Transaction()
        {
            TransactionType = transactionType,
            AccountNumber = accountNumber,
            DestinationAccountNumber = destinationAccountNumber,
            Amount = amount,
            Comment = comment,
            TransactionTimeUtc = DateTime.UtcNow,
        };
        var errors = ValidateModel(model);
        Assert.True(errors.Count == 0);
    }

    [Theory]
    [InlineData(nameof(Transaction.TransactionType),'Z', 1000, 9999, 0.01, "Hello")]
    [InlineData(nameof(Transaction.AccountNumber),'D', 0001, 9999, 0.01, "Hello")]
    [InlineData(nameof(Transaction.AccountNumber),'D', 10000, 9999, 0.01, "Hello")]
    [InlineData(nameof(Transaction.DestinationAccountNumber),'D', 1000, 0001, 0.01, "Hello")]
    [InlineData(nameof(Transaction.DestinationAccountNumber),'D', 1000, 10000, 0.01, "Hello")]
    [InlineData(nameof(Transaction.Amount),'D', 1000, 9999, 0, "Hello")]
    [InlineData(nameof(Transaction.Amount),'D', 1000, 9999, -1, "Hello")]
    [InlineData(nameof(Transaction.Comment),'D', 1000, 9999, 0.01, "1234567890123456789012345678901")]
    public void CreateModel_InvalidParameter(string memberName,char transactionType, int accountNumber, int? destinationAccountNumber, decimal amount, string? comment)
    {
        var model = new Transaction()
        {
            TransactionType = transactionType,
            AccountNumber = accountNumber,
            DestinationAccountNumber = destinationAccountNumber,
            Amount = amount,
            Comment = comment,
            TransactionTimeUtc = DateTime.UtcNow,
        };
        var errors = ValidateModel(model);
        Assert.True(errors.Count == 1);
        Assert.True(errors.FirstOrDefault()!.MemberNames.FirstOrDefault() == memberName);
    }

    private IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var ctx = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, ctx, validationResults, true);
        return validationResults;
    }
}
