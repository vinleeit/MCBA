using Mcba.Middlewares;
using Mcba.Services;
using Mcba.Services.Interfaces;
using Mcba.ViewModels.Withdraw;
using Microsoft.AspNetCore.Mvc;

namespace Mcba.Controllers;

public class WithdrawController(IAccountService accountService, IBalanceService balanceService, IWithdrawService withdrawService) : Controller
{
    private readonly IAccountService _accountService = accountService;
    private readonly IBalanceService _balanceService = balanceService;
    private readonly IWithdrawService _withdrawService = withdrawService;

    [LoggedIn]
    public async Task<IActionResult> Index()
    {
        int? customerID = HttpContext.Session.GetInt32("Customer");
        List<Models.Account> accounts = await _accountService.GetAccounts(
            customerID.GetValueOrDefault()
        );

        return View(new WithdrawViewModel() { Accounts = accounts });
    }

    [HttpPost]
    [LoggedIn]
    public async Task<IActionResult> Index([FromForm] WithdrawViewModel data)
    {
        if (!ModelState.IsValid)
        {
            int? customerID = HttpContext.Session.GetInt32("Customer");
            List<Models.Account> accounts = await _accountService.GetAccounts(
                customerID.GetValueOrDefault()
            );
            data.Accounts = accounts;
            return View(data);
        }
        var balance = await _balanceService.GetAccountBalance(data.AccountNumber.GetValueOrDefault());
        var (totalAmount, minimumBalance) = await _withdrawService.GetTotalAmountAndMinimumAllowedBalance(data.AccountNumber.GetValueOrDefault(), data.Amount);
        if (totalAmount > balance - minimumBalance)
        {
            ModelState.AddModelError("Amount", $"Balance for account {data.AccountNumber}(${balance:f2}) is not enough to draw ${totalAmount:f2}. minimum balance is ${minimumBalance:f2}");
            int? customerID = HttpContext.Session.GetInt32("Customer");
            List<Models.Account> accounts = await _accountService.GetAccounts(
                customerID.GetValueOrDefault()
            );
            return View(new WithdrawViewModel() { Accounts = accounts });
        }

        return View("ConfirmDeposit", data);
    }

    [HttpPost]
    [LoggedIn]
    public async Task<IActionResult> WithdrawConfirmed([FromForm] WithdrawViewModel data)
    {
        var result = await _withdrawService.Withdraw(data.AccountNumber.GetValueOrDefault(), data.Amount, data.Comment);

        ViewBag.Error = false;
        if (result != null)
        {
            ViewBag.Error = true;
            return View("Result");
        }
        return View("Result");

    }
}
