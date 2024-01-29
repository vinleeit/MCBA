using System.Diagnostics;
using Mcba.Middlewares;
using Mcba.Services.Interfaces;
using Mcba.ViewModels;
using Mcba.ViewModels.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace Mcba.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBalanceService _balanceService;
    private readonly IAccountService _accountService;

    public HomeController(
        ILogger<HomeController> logger,
        IBalanceService balanceService,
        IAccountService accountService
    )
    {
        _logger = logger;
        _accountService = accountService;
        _balanceService = balanceService;
    }

    public async Task<IActionResult> Index()
    {
        var customerID = HttpContext.Session.GetInt32("Customer");
        if (!customerID.HasValue)
        {
            return RedirectToAction("Login", "Auth");
        }
        var data = new DashboardViewModel() { Balances = [] };
        var accounts = await _accountService.GetAccounts(customerID.Value);
        foreach (var a in accounts)
        {
            var balance = await _balanceService.GetAccountBalance(a.AccountNumber);
            data.Balances.Add((a.AccountNumber, balance));
        }
        return View(data);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
