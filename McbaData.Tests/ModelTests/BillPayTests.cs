namespace McbaData.Tests.ModelTests;

using System.ComponentModel.DataAnnotations;
using McbaData.Models;
using Xunit;

public class BillPayTests
{
    [Theory]
    [InlineData(0, 1000, 0.01, 'O')]
    [InlineData(1, 1000, 0.1, 'M')]
    [InlineData(2, 9999, 1, 'M')]
    public void CreateModel_ValidParameter(int payeeID, int accountNumber, decimal amount, char period)
    {
        var model = new BillPay()
        {
            PayeeID = payeeID,
            AccountNumber = accountNumber,
            Amount = amount,
            ScheduleTimeUtc = DateTime.UtcNow,
            Period = period,
        };
        var errors = ValidateModel(model);
        Assert.True(errors.Count == 0);
    }

    [Theory]
    [InlineData(nameof(BillPay.PayeeID), -1, 1000, 1, 'O')]
    [InlineData(nameof(BillPay.AccountNumber), 0, 0001, 1, 'O')]
    [InlineData(nameof(BillPay.AccountNumber), 0, 10000, 1, 'O')]
    [InlineData(nameof(BillPay.Amount), 0, 1000, -1, 'O')]
    [InlineData(nameof(BillPay.Amount), 0, 1000, 0, 'O')]
    [InlineData(nameof(BillPay.Period), 0, 1000, 1, 'A')]
    public void CreateModel_InvalidParameter(string memberName, int payeeID, int accountNumber, decimal amount, char period)
    {
        var model = new BillPay()
        {
            PayeeID = payeeID,
            AccountNumber = accountNumber,
            Amount = amount,
            ScheduleTimeUtc = DateTime.UtcNow,
            Period = period,
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
