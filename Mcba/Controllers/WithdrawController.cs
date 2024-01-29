using Mcba.Middlewares;
using Mcba.Services.Interfaces;
using Mcba.ViewModels.Withdraw;
using Microsoft.AspNetCore.Mvc;

namespace Mcba.Controllers;

[LoggedIn]
public class WithdrawController(
    IAccountService accountService,
    IBalanceService balanceService,
    IWithdrawService withdrawService
) : Controller
{
    private readonly IAccountService _accountService = accountService;
    private readonly IBalanceService _balanceService = balanceService;
    private readonly IWithdrawService _withdrawService = withdrawService;

    public async Task<IActionResult> Index()
    {
        int? customerID = HttpContext.Session.GetInt32("Customer");
        List<McbaData.Models.Account> accounts = await _accountService.GetAccounts(
            customerID.GetValueOrDefault()
        );

        return View(new WithdrawViewModel() { Accounts = accounts });
    }

    [HttpPost]
    public async Task<IActionResult> Index([FromForm] WithdrawViewModel data)
    {
        if (!ModelState.IsValid)
        {
            int? customerID = HttpContext.Session.GetInt32("Customer");
            List<McbaData.Models.Account> accounts = await _accountService.GetAccounts(
                customerID.GetValueOrDefault()
            );
            data.Accounts = accounts;
            return View(data);
        }
        var balance = await _balanceService.GetAccountBalance(
            data.AccountNumber.GetValueOrDefault()
        );
        var (totalAmount, minimumBalance) =
            await _withdrawService.GetTotalAmountAndMinimumAllowedBalance(
                data.AccountNumber.GetValueOrDefault(),
                data.Amount.GetValueOrDefault()
            );
        if (totalAmount > balance - minimumBalance)
        {
            ModelState.AddModelError(
                "Amount",
                $"Account {data.AccountNumber} has insufficient balance (${balance:f2}) to draw ${totalAmount} (with service charge); the minimum balance is ${minimumBalance:f2}"
            );
            int? customerID = HttpContext.Session.GetInt32("Customer");
            List<McbaData.Models.Account> accounts = await _accountService.GetAccounts(
                customerID.GetValueOrDefault()
            );
            return View(new WithdrawViewModel() { Accounts = accounts });
        }

        return View("ConfirmDeposit", data);
    }

    [HttpPost]
    public async Task<IActionResult> WithdrawConfirmed([FromForm] WithdrawViewModel data)
    {
        var result = await _withdrawService.Withdraw(
            data.AccountNumber.GetValueOrDefault(),
            data.Amount.GetValueOrDefault(),
            data.Comment
        );

        ViewBag.Error = false;
        if (result != null)
        {
            ViewBag.Error = true;
            return View("Result");
        }
        return View("Result");
    }

    public IActionResult WithdrawCanceled()
    {
        return RedirectToAction(nameof(Index));
    }
}
