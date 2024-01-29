namespace McbaData.Tests.ModelTests;

using System.ComponentModel.DataAnnotations;
using McbaData.Models;
using Xunit;

public class PayeeTests
{
    [Theory]
    [InlineData("Matthew", "address", "city", "NSW", "1234", "(04) 7212 3456")]
    [InlineData("Matthew", "address", "city", "VIC", "1234", "(04) 7212 3456")]
    [InlineData("Matthew", "address", "city", "QLD", "1234", "(04) 7212 3456")]
    [InlineData("Matthew", "address", "city", "WA", "1234", "(04) 7212 3456")]
    [InlineData("Matthew", "address", "city", "SA", "1234", "(04) 7212 3456")]
    [InlineData("Matthew", "address", "city", "TAS", "1234", "(04) 7212 3456")]
    [InlineData("Matthew", "address", "city", "ACT", "1234", "(04) 7212 3456")]
    [InlineData("Matthew", "address", "city", "NT", "1234", "(04) 7212 3456")]
    public void CreateModel_ValidParameter(string name, string address, string city, string state, string postcode, string phone)
    {
        var model = new Payee()
        {
            Name = name,
            Address = address,
            City = city,
            State = state,
            Postcode = postcode,
            Phone = phone,
        };
        var errors = ValidateModel(model);
        Assert.True(errors.Count == 0);
    }

    [Theory]
    [InlineData(nameof(Payee.Name), "", "address", "city", "NSW", "1234", "(04) 7212 3456")]
    [InlineData(nameof(Payee.Name), "123456789012345678901234567890123456789012345678901", "address", "city", "NSW", "1234", "(04) 7212 3456")]
    [InlineData(nameof(Payee.Address), "Matthew", "", "city", "NSW", "1234", "(04) 7212 3456")]
    [InlineData(nameof(Payee.Address), "Matthew", "123456789012345678901234567890123456789012345678901", "city", "NSW", "1234", "(04) 7212 3456")]
    [InlineData(nameof(Payee.City), "Matthew", "address", "", "NSW", "1234", "(04) 7212 3456")]
    [InlineData(nameof(Payee.City), "Matthew", "address", "12345678901234567890123456789012345678901", "NSW", "1234", "(04) 7212 3456")]
    [InlineData(nameof(Payee.State), "Matthew", "address", "city", "", "1234", "(04) 7212 3456")]
    [InlineData(nameof(Payee.State), "Matthew", "address", "city", "LIE", "1234", "(04) 7212 3456")]
    [InlineData(nameof(Payee.Postcode), "Matthew", "address", "city", "NSW", "", "(04) 7212 3456")]
    [InlineData(nameof(Payee.Postcode), "Matthew", "address", "city", "NSW", "123", "(04) 7212 3456")]
    [InlineData(nameof(Payee.Postcode), "Matthew", "address", "city", "NSW", "12345", "(04) 7212 3456")]
    [InlineData(nameof(Payee.Phone), "Matthew", "address", "city", "NSW", "1234", "")]
    [InlineData(nameof(Payee.Phone), "Matthew", "address", "city", "NSW", "1234", "0472123456")]
    [InlineData(nameof(Payee.Phone), "Matthew", "address", "city", "NSW", "1234", "04 72ab cdef")]
    public void CreateModel_InvalidParameter(string memberName, string name, string address, string city, string state, string postcode, string phone)
    {
        var model = new Payee()
        {
            Name = name,
            Address = address,
            City = city,
            State = state,
            Postcode = postcode,
            Phone = phone,
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
