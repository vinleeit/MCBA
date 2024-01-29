using System.Text.Json;
using Mcba.Middlewares;
using Mcba.Services.Interfaces;
using Mcba.ViewModels.Deposit;
using McbaData.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mcba.Controllers;

[LoggedIn]
public class DepositController(IAccountService accountService, IDepositService depositService)
    : Controller
{
    private readonly IAccountService _accountService = accountService;
    private readonly IDepositService _depositService = depositService;

    public async Task<IActionResult> Index()
    {
        int? customerID = HttpContext.Session.GetInt32("Customer");
        // Get accounts from customer ID
        List<Account> accounts = await _accountService.GetAccounts(customerID.GetValueOrDefault());
        byte[] serialized = JsonSerializer.SerializeToUtf8Bytes(accounts);
        // Save serialized account to session
        HttpContext.Session.Set("accounts", serialized);
        return View(new DepositViewModel() { Accounts = accounts });
    }

    [HttpPost]
    public IActionResult Index([FromForm] DepositViewModel data)
    {
        if (!ModelState.IsValid)
        {
            // Get accounts from session
            byte[]? serializedList = HttpContext.Session.Get("accounts");
            List<Account>? accounts = JsonSerializer.Deserialize<List<Account>>(serializedList);
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
            data.Amount.GetValueOrDefault(),
            data.Comment
        );
        ViewBag.Error = false;
        if (result != null)
        {
            ViewBag.Error = true;
        }

        return View("Result");
    }

    public IActionResult DepositCanceled()
    {
        return RedirectToAction(nameof(Index));
    }
}
