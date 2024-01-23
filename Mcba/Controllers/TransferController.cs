using Mcba.Middlewares;
using Mcba.Services.Interfaces;
using Mcba.ViewModels.Transfer;
using McbaData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mcba.Controllers;

public class TransferController(
    IAccountService accountService,
    ITransferService transferService,
    McbaContext context,
    IBalanceService balanceService
) : Controller
{
    private readonly IAccountService _accountService = accountService;
    private readonly ITransferService _transferService = transferService;
    private readonly IBalanceService _balanceService = balanceService;
    private readonly McbaContext _dbContext = context;

    [LoggedIn]
    public async Task<IActionResult> Index()
    {
        int customerID = HttpContext.Session.GetInt32("Customer")!.Value;
        var accounts = await _accountService.GetAccounts(customerID);
        return View(new TransferViewModel() { Accounts = accounts });
    }

    [HttpPost]
    [LoggedIn]
    public async Task<IActionResult> Index([FromForm] TransferViewModel data)
    {
        if (!ModelState.IsValid)
        {
            int customerID = HttpContext.Session.GetInt32("Customer")!.Value;
            var accounts = await _accountService.GetAccounts(customerID);
            data.Accounts = accounts;
            return View(data);
        }
        // Convert destination account number to int
        if (!Int32.TryParse(data.DestinationAccountNumber, out int destNumber))
        {
            // Cannot convert to int
            int customerID = HttpContext.Session.GetInt32("Customer")!.Value;
            var accounts = await _accountService.GetAccounts(customerID);
            data.Accounts = accounts;
            ModelState.AddModelError(
                "DestinationAccountNumber",
                "Destination account number is not a valid digit"
            );
            return View(data);
        }
        if (destNumber == data.AccountNumber)
        {
            int customerID = HttpContext.Session.GetInt32("Customer")!.Value;
            var accounts = await _accountService.GetAccounts(customerID);
            data.Accounts = accounts;
            ModelState.AddModelError(
                "DestinationAccountNumber",
                "Destination account number cannot be the same as source"
            );
            return View(data);
        }
        // Check if destination is valid
        if (
            await (
                from a in _dbContext.Accounts
                where a.AccountNumber == destNumber
                select a
            ).CountAsync() < 1
        )
        {
            int customerID = HttpContext.Session.GetInt32("Customer")!.Value;
            var accounts = await _accountService.GetAccounts(customerID);
            data.Accounts = accounts;
            ModelState.AddModelError(
                "DestinationAccountNumber",
                "Destination account number is not found"
            );
            return View(data);
        }
        // Check if balance is enough
        var balance = await _balanceService.GetAccountBalance(
            data.AccountNumber.GetValueOrDefault()
        );
        var (total, minimum) = await _transferService.GetTotalAndMinimumBalance(
            data.AccountNumber.GetValueOrDefault(),
            data.Amount
        );
        if (balance - minimum < total)
        {
            int customerID = HttpContext.Session.GetInt32("Customer")!.Value;
            var accounts = await _accountService.GetAccounts(customerID);
            data.Accounts = accounts;
            ModelState.AddModelError(
                "Amount",
                $"Balance for account {data.AccountNumber}(${balance:f2}) is not enough to transfer ${total:f2}. minimum balance is ${minimum:f2}"
            );
            return View(data);
        }

        return View("ConfirmTransfer", data);
    }

    [HttpPost]
    [LoggedIn]
    public async Task<IActionResult> TransferConfirmed([FromForm] TransferViewModel data)
    {
        int destNum = Int32.Parse(data.DestinationAccountNumber!);
        var result = await _transferService.Transfer(
            data.AccountNumber.GetValueOrDefault(),
            destNum,
            data.Amount,
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
}

