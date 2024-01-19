using Mcba.Data.JsonModel;
using Mcba.Models;
using Newtonsoft.Json;

namespace Mcba.Data
{
    public static class DataLoader
    {
        // Data JSON URL
        private const string _jsonResourceURI =
            "https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/";

        public static async Task<bool> SeedData(McbaContext context)
        {
            // Check if the information is empty in database
            if (context.Customers.Any())
            {
                return true;
            }

            // Load data from webserver json
            string? jsonDataString;
            try
            {
                using HttpResponseMessage response = await new HttpClient().GetAsync(
                    _jsonResourceURI
                );
                _ = response.EnsureSuccessStatusCode();
                // Read data as string
                jsonDataString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            if (jsonDataString == null)
            {
                return false;
            }

            // Convert String to json using Newtonsoft.JSON
            IList<CustomerJsonDTO>? jsonData = JsonConvert.DeserializeObject<
                IList<CustomerJsonDTO>
            >(
                jsonDataString!,
                // Set custom date format
                new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy hh:mm:ss tt" }
            );
            if (jsonData == null)
            {
                return false;
            }

            // Start a db transaction
            using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction dbTransaction =
                context.Database.BeginTransaction();
            // Save Customer to database
            foreach (CustomerJsonDTO customer in jsonData)
            {
                Customer customer1 =
                    new()
                    {
                        CustomerID = customer.CustomerID,
                        Name = customer.Name,
                        Address = customer.Address,
                        Postcode = customer.PostCode,
                        City = customer.City
                    };
                try
                {
                    _ = await context.Customers.AddAsync(customer1);
                    int affected = await context.SaveChangesAsync();
                    if (affected < 0)
                    {
                        await dbTransaction.RollbackAsync();
                        return false;
                    }
                }
                catch (Exception)
                {
                    await dbTransaction.RollbackAsync();
                    return false;
                }
                foreach (AccountJsonDTO account in customer.Accounts)
                {
                    List<Transaction> transactions = [];
                    foreach (TransactionJsonDTO currentTransaction in account.Transactions)
                    {
                        transactions.Add(
                            new Transaction()
                            {
                                TransactionTimeUtc = currentTransaction.TransactionTimeUtc,
                                Amount = (decimal)currentTransaction.Amount,
                                AccountNumber = account.AccountNumber,
                                DestinationAccountNumber = null,
                                TransactionType = 'D',
                                Comment = currentTransaction.Comment,
                            }
                        );
                    }
                    Account currentAccount =
                        new()
                        {
                            AccountNumber = account.AccountNumber,
                            CustomerID = account.CustomerID,
                            AccountType = account.AccountType[0],
                            Transactions = transactions
                        };

                    try
                    {
                        _ = await context.Accounts.AddAsync(currentAccount);
                        int affected = await context.SaveChangesAsync();
                        if (affected < 0)
                        {
                            await dbTransaction.RollbackAsync();
                            return false;
                        }
                    }
                    catch (Exception)
                    {
                        await dbTransaction.RollbackAsync();
                        return false;
                    }
                }

                Login login =
                    new()
                    {
                        CustomerID = customer.CustomerID,
                        LoginID = customer.Login.LoginID,
                        PasswordHash = customer.Login.PasswordHash,
                    };
                try
                {
                    _ = await context.Logins.AddAsync(login);
                    int affected = await context.SaveChangesAsync();
                    if (affected < 0)
                    {
                        await dbTransaction.RollbackAsync();
                        return false;
                    }
                }
                catch (Exception)
                {
                    await dbTransaction.RollbackAsync();
                    return false;
                }
            }
            await dbTransaction.CommitAsync();
            return true;
        }
    }
}
