using Mcba.Data;
using Mcba.Middlewares;
using Mcba.Services;
using Mcba.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<McbaContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("arvin-rmit"))
);
builder.Services.AddScoped<IDepositService, DepositService>();
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

var app = builder.Build();

// Seed database if data is empty
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetService<McbaContext>();
    await DataLoader.SeedData(context!);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseMiddleware<AuthorizationMiddleware>();

/* app.UseAuthorization(); */

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
