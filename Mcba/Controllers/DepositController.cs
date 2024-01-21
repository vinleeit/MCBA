using System.Text.Json;
using Mcba.Data;
using Mcba.Middlewares;
using Mcba.Models;
using Mcba.Services.Interfaces;
using Mcba.ViewModels.Deposit;
using Microsoft.AspNetCore.Mvc;

namespace Mcba.Controllers;

public class DepositController(IAccountService accountService, IDepositService depositService)
    : Controller
{
    private readonly IAccountService _accountService = accountService;
    private readonly IDepositService _depositService = depositService;

    [LoggedIn]
    public async Task<IActionResult> Index()
    {
        var customerID = HttpContext.Session.GetInt32("Customer");
        var accounts = await _accountService.GetAccounts(customerID.GetValueOrDefault());
        var serialized = JsonSerializer.SerializeToUtf8Bytes(accounts);
        HttpContext.Session.Set("accounts", serialized);
        return View(new DepositViewModel() { Accounts = accounts });
    }

    [HttpPost]
    [LoggedIn]
    public IActionResult Index([FromForm] DepositViewModel data)
    {
        if (!ModelState.IsValid)
        {
            var serializedList = HttpContext.Session.Get("accounts");
            var accounts = JsonSerializer.Deserialize<List<Account>>(serializedList);
            data.Accounts = accounts ?? ([]);
            return View(data);
        }
        // Only remove account cache once user input is okay
        HttpContext.Session.Remove("accounts");
        return View("ConfirmDeposit", data);
    }

    [HttpPost]
    public async Task<IActionResult> DepositConfirmed([FromForm] DepositViewModel data)
    {
        IDepositService.DepositError? result = await _depositService.Deposit(
            data.AccountNumber.GetValueOrDefault(),
            data.Amount,
            data.Comment
        );
        ViewBag.Error = false;
        if (result != null)
        {
            ViewBag.Error = true;
        }

        return View("Result");
    }
}
