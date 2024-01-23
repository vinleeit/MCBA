using McbaData.Models;
using Microsoft.EntityFrameworkCore;

namespace McbaData;

public class McbaContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Login> Logins { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Payee> Payees { get; set; }
    public DbSet<BillPay> BillPays { get; set; }
}
