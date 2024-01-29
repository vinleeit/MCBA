using McbaData;
using McbaData.Models;
using Microsoft.Extensions.DependencyInjection;

public class DataSeeder
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<McbaContext>();

        Initialize(context);
    }

    public static void Initialize(McbaContext context)
    {
        if (!context.Customers.Any())
        {
            context.Customers.AddRange([
                new Customer
                    {
                        CustomerID = 1000,
                        Name = "John Doe",
                        TFN = "123 456 789",
                        Address = "123 Main St",
                        City = "Melbourne",
                        State = "VIC",
                        Postcode = "3000",
                        Mobile = "0412 345 678"
                    },
                new Customer
                {
                    CustomerID = 1001,
                    Name = "Jane Doe",
                    TFN = "987 654 321",
                    Address = "456 Park Ave",
                    City = "Sydney",
                    State = "NSW",
                    Postcode = "2000",
                    Mobile = "0434 567 890"
                },
            ]);
        }

        if (!context.Accounts.Any())
        {
            context.Accounts.AddRange([
                new Account
                {
                    AccountNumber = 1000,
                    AccountType = 'C',
                    CustomerID = 1000
                },
                new Account
                {
                    AccountNumber = 1001,
                    AccountType = 'S',
                    CustomerID = 1000
                },
                new Account
                {
                    AccountNumber = 1002,
                    AccountType = 'C',
                    CustomerID = 1001
                },
            ]);
        }

        if (!context.Logins.Any())
        {
            context.Logins.AddRange([
                new Login
                {
                    LoginID = "10000000",
                    CustomerID = 1000,
                    PasswordHash = "94characterpasswordhash"
                },
                new Login
                {
                    LoginID = "10000001",
                    CustomerID = 1001,
                    PasswordHash = "94characterpasswordhash"
                }
            ]);
        }

        if (!context.Payees.Any())
        {
            context.Payees.AddRange([
                new Payee()
                {
                    Name = "Telstra",
                    Address = "Bourke St",
                    City = "Melbourne",
                    State = "VIC",
                    Postcode = "0001",
                    Phone = "(04) 0072 6917",
                },
                new Payee()
                {
                    Name = "Greater Water Australia",
                    Address = "1 McNab Ave",
                    City = "Footscray",
                    State = "VIC",
                    Postcode = "0002",
                    Phone = "(04) 0013 4499",
                },
                new Payee()
                {
                    Name = "Red Energy",
                    Address = "570 Church St",
                    City = "Cremorne",
                    State = "VIC",
                    Postcode = "0003",
                    Phone = "(04) 0013 1806",
                },
            ]);
        }

        if (!context.BillPays.Any())
        {
            context.BillPays.AddRange([
                new BillPay
                {
                    BillPayID = 1,
                    AccountNumber = 1000,
                    PayeeID = 1,
                    Amount = 125.50m,
                    ScheduleTimeUtc = DateTime.UtcNow,
                    Period = 'M'
                }
            ]);
        }

        if (!context.Transactions.Any())
        {
            context.AddRange([
                new Transaction
                {
                    TransactionID = 1,
                    TransactionType = 'D',
                    AccountNumber = 1000,
                    Amount = 1000,
                    TransactionTimeUtc = DateTime.UtcNow
                },
                new Transaction
                {
                    TransactionID = 2,
                    TransactionType = 'D',
                    AccountNumber = 1001,
                    Amount = 1000,
                    TransactionTimeUtc = DateTime.UtcNow
                },
                new Transaction
                {
                    TransactionID = 3,
                    TransactionType = 'D',
                    AccountNumber = 1002,
                    Amount = 1000,
                    TransactionTimeUtc = DateTime.UtcNow
                },
            ]);
        }
        context.SaveChanges();
    }
}