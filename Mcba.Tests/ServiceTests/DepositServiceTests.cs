using Mcba.Services;
using McbaData;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static Mcba.Services.Interfaces.IDepositService;

public class DepositServiceTests : IDisposable
{
    private readonly McbaContext _dbContext;

    public DepositServiceTests()
    {
        _dbContext = new McbaContext(
            new DbContextOptionsBuilder<McbaContext>().
            UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared").Options
        );
        _dbContext.Database.EnsureCreated();
        DataSeeder.Initialize(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }

    [Theory]
    [InlineData(1000, 10, "My new deposit")]
    [InlineData(1001, 15, "")]
    [InlineData(1002, 20, null)]
    public async Task Deposit_Success_SaveToDB(int accountNumber, decimal amount, string? comment)
    {
        var depositService = new DepositService(_dbContext);
        var error = await depositService.Deposit(
            accountNumber,
            amount,
            comment
        );

        Assert.True(error == null);

        // Last transaction is the newest appended
        var transaction = _dbContext.Transactions.OrderBy(e => e.TransactionID).ToList().Last();
        Assert.True(transaction.AccountNumber == accountNumber);
        Assert.True(transaction.Amount == amount);
        Assert.Equal(transaction.Comment, comment);
        Assert.True(transaction.TransactionType == 'D');
        Assert.True(transaction.TransactionTimeUtc.Subtract(DateTime.UtcNow).Minutes <= 5);
    }

    [Theory]
    [InlineData(DepositError.InvalidAmount, 1000, 0, "My new deposit")]
    [InlineData(DepositError.Unknown, 999, 10, null)]
    [InlineData(DepositError.Unknown, 1004, 10, null)]
    public async Task Deposit_Failed_SaveToDB(DepositError? expectedError, int accountNumber, decimal amount, string? comment)
    {
        var depositService = new DepositService(_dbContext);
        var error = await depositService.Deposit(
            accountNumber,
            amount,
            comment
        );

        Assert.Equal(expectedError, error);
    }
}