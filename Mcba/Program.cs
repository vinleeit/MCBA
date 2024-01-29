using Hangfire;
using Mcba.Data;
using Mcba.Middlewares;
using Mcba.Services;
using Mcba.Services.Interfaces;
using McbaData;
using Microsoft.EntityFrameworkCore;
using Tailwind;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add database context configuration
builder.Services.AddDbContext<McbaContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("arvin-rmit"),
            b => b.MigrationsAssembly("Mcba")
        )
);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
});

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IFreeTransactionService, FreeTransactionService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<IWithdrawService, WithdrawService>();
builder.Services.AddScoped<ITransferService, TransferService>();
builder.Services.AddScoped<IStatementService, StatementService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IDepositService, DepositService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IStatementService, StatementService>();

// Add hangfire background service
builder.Services.AddHangfire(
    (prov, conf) =>
    {
        AutomaticRetryAttribute retryAttr = new() { Attempts = 0 };
        _ = conf.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseFilter(retryAttr)
            .UseSqlServerStorage(builder.Configuration.GetConnectionString("arvin-rmit"));
    }
);
builder.Services.AddHangfireServer();
builder.Services.AddScoped<IBillPayService, BillPayService>();

var app = builder.Build();

// Seed database if data is empty
using (IServiceScope scope = app.Services.CreateScope())
{
    McbaContext? context = scope.ServiceProvider.GetService<McbaContext>();
    _ = await DataLoader.SeedData(context!);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Display Hangfire dashboard only in development mode.
    app.UseHangfireDashboard();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

// Add middleware that prevent access to protected routes
app.UseMiddleware<AuthorizationMiddleware>();

/* app.UseAuthorization(); */

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

if (app.Environment.IsDevelopment())
{
    // Run tailwind css builder to perform asynchronous JIT build and minification of CSS
    app.RunTailwind("tailwind", "./");
}

app.Run();
