namespace McbaData.Tests.ModelTests;

using System.ComponentModel.DataAnnotations;
using McbaData.Models;
using Xunit;

public class LoginTests
{
    [Theory]
    [InlineData("12345678", 1000, "Rfc2898DeriveBytes$50000$fB5lteA+LLB0mKVz9EBA7A==$Tx0nXJ8aJjBU/mS2ssFIMs3m7vaiyitRmBRvBAYWauw=", null)]
    [InlineData("12345678", 9999, "Rfc2898DeriveBytes$50000$fB5lteA+LLB0mKVz9EBA7A==$Tx0nXJ8aJjBU/mS2ssFIMs3m7vaiyitRmBRvBAYWauw=", null)]
    [InlineData("12345678", 1000, "Rfc2898DeriveBytes$50000$fB5lteA+LLB0mKVz9EBA7A==$Tx0nXJ8aJjBU/mS2ssFIMs3m7vaiyitRmBRvBAYWauw=", true)]
    public void CreateModel_ValidParameter(string loginID, int customerID, string passwordHash, bool? isLocked)
    {
        var model = (!isLocked.HasValue) ? new Login()
        {
            LoginID = loginID,
            CustomerID = customerID,
            PasswordHash = passwordHash,
        } : new Login()
        {
            LoginID = loginID,
            CustomerID = customerID,
            PasswordHash = passwordHash,
            Locked = isLocked.Value
        };
        var errors = ValidateModel(model);
        Assert.True(errors.Count == 0);
        if (!isLocked.HasValue || !isLocked.Value)
        {
            Assert.False(model.Locked);
        }
        else
        {
            Assert.True(model.Locked);
        }
    }

    [Theory]
    [InlineData(nameof(Login.LoginID), "", 1000, "Rfc2898DeriveBytes$50000$fB5lteA+LLB0mKVz9EBA7A==$Tx0nXJ8aJjBU/mS2ssFIMs3m7vaiyitRmBRvBAYWauw=")]
    [InlineData(nameof(Login.LoginID), "abcdefgh", 1000, "Rfc2898DeriveBytes$50000$fB5lteA+LLB0mKVz9EBA7A==$Tx0nXJ8aJjBU/mS2ssFIMs3m7vaiyitRmBRvBAYWauw=")]
    [InlineData(nameof(Login.LoginID), "123456789", 1000, "Rfc2898DeriveBytes$50000$fB5lteA+LLB0mKVz9EBA7A==$Tx0nXJ8aJjBU/mS2ssFIMs3m7vaiyitRmBRvBAYWauw=")]
    [InlineData(nameof(Login.CustomerID), "12345678", 0001, "Rfc2898DeriveBytes$50000$fB5lteA+LLB0mKVz9EBA7A==$Tx0nXJ8aJjBU/mS2ssFIMs3m7vaiyitRmBRvBAYWauw=")]
    [InlineData(nameof(Login.CustomerID), "12345678", 10000, "Rfc2898DeriveBytes$50000$fB5lteA+LLB0mKVz9EBA7A==$Tx0nXJ8aJjBU/mS2ssFIMs3m7vaiyitRmBRvBAYWauw=")]
    [InlineData(nameof(Login.PasswordHash), "12345678", 1000, "")]
    [InlineData(nameof(Login.PasswordHash), "12345678", 1000, "Rfc2898DeriveBytes$50000$fB5lteA+LLB0mKVz9EBA7A==$Tx0nXJ8aJjBU/mS2ssFIMs3m7vaiyitRmBRvBAYWauw=1")]
    public void CreateModel_InvalidParameter(string memberName, string loginID, int customerID, string passwordHash)
    {
        var model = new Login()
        {
            LoginID = loginID,
            CustomerID = customerID,
            PasswordHash = passwordHash
        };
        var errors = ValidateModel(model);
        Assert.True(errors.Count == 1);
        Assert.True(errors.FirstOrDefault()!.MemberNames.FirstOrDefault() == memberName);
    }

    private IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }
}
