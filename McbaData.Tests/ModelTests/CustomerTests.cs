namespace McbaData.Tests.ModelTests;

using System.ComponentModel.DataAnnotations;
using McbaData.Models;
using Xunit;

public class CustomerTests
{
    [Theory]
    [InlineData(1000, "Matthew", null, null, null, null, null, null)]
    [InlineData(1000, "Matthew", "", "", "", "", "", "")]
    [InlineData(1000, "Matthew", "123 456 789", "address", "city", "NSW", "1234", "0472 123 456")]
    [InlineData(1000, "Matthew", null, null, null, "VIC", null, null)]
    [InlineData(1000, "Matthew", null, null, null, "QLD", null, null)]
    [InlineData(1000, "Matthew", null, null, null, "WA", null, null)]
    [InlineData(1000, "Matthew", null, null, null, "SA", null, null)]
    [InlineData(1000, "Matthew", null, null, null, "TAS", null, null)]
    [InlineData(1000, "Matthew", null, null, null, "ACT", null, null)]
    [InlineData(1000, "Matthew", null, null, null, "NT", null, null)]
    public void CreateModel_ValidParameter(int customerID, string name, string? tfn, string? address, string? city, string? state, string? postcode, string? mobile)
    {
        var model = new Customer()
        {
            CustomerID = customerID,
            Name = name,
            TFN = tfn,
            Address = address,
            City = city,
            State = state,
            Postcode = postcode,
            Mobile = mobile,
        };
        var errors = ValidateModel(model);
        Assert.True(errors.Count == 0);
    }

    [Theory]
    [InlineData(nameof(Customer.CustomerID), 0001, "Matthew", null, null, null, null, null, null)]
    [InlineData(nameof(Customer.CustomerID), 10000, "Matthew", null, null, null, null, null, null)]
    [InlineData(nameof(Customer.Name), 1000, "", null, null, null, null, null, null)]
    [InlineData(nameof(Customer.Name), 1000, "123456789012345678901234567890123456789012345678901", null, null, null, null, null, null)]
    [InlineData(nameof(Customer.TFN), 1000, "Matthew", "abc def ghi", null, null, null, null, null)]
    [InlineData(nameof(Customer.TFN), 1000, "Matthew", "123456789", null, null, null, null, null)]
    [InlineData(nameof(Customer.Address), 1000, "Matthew", null, "123456789012345678901234567890123456789012345678901", null, null, null, null)]
    [InlineData(nameof(Customer.City), 1000, "Matthew", null, null, "12345678901234567890123456789012345678901", null, null, null)]
    [InlineData(nameof(Customer.State), 1000, "Matthew", null, null, null, "LIE", null, null)]
    [InlineData(nameof(Customer.Postcode), 1000, "Matthew", null, null, null, null, "12345", null)]
    [InlineData(nameof(Customer.Postcode), 1000, "Matthew", null, null, null, null, "123", null)]
    [InlineData(nameof(Customer.Mobile), 1000, "Matthew", null, null, null, null,null, "0472123456")]
    [InlineData(nameof(Customer.Mobile), 1000, "Matthew", null, null, null, null,null, "0472 abc def")]
    public void CreateModel_InvalidParameter(String memberName, int customerID, string name, string? tfn, string? address, string? city, string? state, string? postcode, string? mobile)
    {
        var model = new Customer()
        {
            CustomerID = customerID,
            Name = name,
            TFN = tfn,
            Address = address,
            City = city,
            State = state,
            Postcode = postcode,
            Mobile = mobile,
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
