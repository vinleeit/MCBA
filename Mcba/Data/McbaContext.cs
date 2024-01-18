using Mcba.Models;
using Microsoft.EntityFrameworkCore;

namespace Mcba.Data;

public class McbaContext : DbContext
{
    public McbaContext(DbContextOptions options)
        : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Login> Logins { get; set; }
}
