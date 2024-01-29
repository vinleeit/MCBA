namespace McbaData.Tests.ModelTests;

using System.ComponentModel.DataAnnotations;
using McbaData.Models;
using Xunit;

public class AccountTests
{
    [Theory]
    [InlineData(1000, 'C', 1000)]
    [InlineData(1000, 'S', 1000)]
    [InlineData(9999, 'C', 1000)]
    [InlineData(9999, 'S', 9999)]
    public void CreateModel_ValidParameter(int accountNumber, char accountType, int customerID)
    {
        var model = new Account()
        {
            AccountNumber = accountNumber,
            AccountType = accountType,
            CustomerID = customerID,
        };
        var errors = ValidateModel(model);
        Assert.True(errors.Count == 0);
    }

    [Theory]
    [InlineData(nameof(Account.AccountNumber), 0001, 'C', 1000)] // Invalid account number
    [InlineData(nameof(Account.AccountNumber), 10000, 'C', 1000)] // Invalid account number
    [InlineData(nameof(Account.CustomerID), 1000, 'C', 0001)] // Invalid customer ID
    [InlineData(nameof(Account.CustomerID), 1000, 'C', 10000)] // Invalid customer ID
    [InlineData(nameof(Account.AccountType), 1000, 'A', 1000)] // Invalid account type
    public void CreateModel_InvalidParameter(string memberName, int accountNumber, char accountType, int customerID)
    {
        var model = new Account()
        {
            AccountNumber = accountNumber,
            AccountType = accountType,
            CustomerID = customerID,
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
