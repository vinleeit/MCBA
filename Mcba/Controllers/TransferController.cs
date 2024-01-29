using Mcba.Middlewares;
using Mcba.Services.Interfaces;
using Mcba.ViewModels.Transfer;
using McbaData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mcba.Controllers;

[LoggedIn]
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

    public async Task<IActionResult> Index()
    {
        int customerID = HttpContext.Session.GetInt32("Customer")!.Value;
        List<McbaData.Models.Account> accounts = await _accountService.GetAccounts(customerID);
        return View(new TransferViewModel() { Accounts = accounts });
    }

    [HttpPost]
    public async Task<IActionResult> Index([FromForm] TransferViewModel data)
    {
        if (!ModelState.IsValid)
        {
            int customerID = HttpContext.Session.GetInt32("Customer")!.Value;
            List<McbaData.Models.Account> accounts = await _accountService.GetAccounts(customerID);
            data.Accounts = accounts;
            return View(data);
        }
        int destNumber = Int32.Parse(data.DestinationAccountNumber!);
        if (destNumber == data.AccountNumber)
        {
            int customerID = HttpContext.Session.GetInt32("Customer")!.Value;
            List<McbaData.Models.Account> accounts = await _accountService.GetAccounts(customerID);
            data.Accounts = accounts;
            ModelState.AddModelError(
                "DestinationAccountNumber",
                "Destination account number must not be the same as account number"
            );
            return View(data);
        }
        // Check if destination is valid
        if (
            !await (
                from a in _dbContext.Accounts
                where a.AccountNumber == destNumber
                select a
            ).AnyAsync()
        )
        {
            int customerID = HttpContext.Session.GetInt32("Customer")!.Value;
            List<McbaData.Models.Account> accounts = await _accountService.GetAccounts(customerID);
            data.Accounts = accounts;
            ModelState.AddModelError(
                "DestinationAccountNumber",
                "Destination account number is not registered"
            );
            return View(data);
        }
        // Check if balance is enough
        decimal balance = await _balanceService.GetAccountBalance(
            data.AccountNumber.GetValueOrDefault()
        );
        (decimal total, decimal minimum) = await _transferService.GetTotalAndMinimumBalance(
            data.AccountNumber.GetValueOrDefault(),
            data.Amount.GetValueOrDefault()
        );
        if (balance - minimum < total)
        {
            int customerID = HttpContext.Session.GetInt32("Customer")!.Value;
            var accounts = await _accountService.GetAccounts(customerID);
            data.Accounts = accounts;
            ModelState.AddModelError(
                "Amount",
                $"Account {data.AccountNumber} has insufficient balance (${balance:f2}) to transfer ${total} (with service charge); the minimum balance is ${minimum:f2}"
            );
            return View(data);
        }

        return View("ConfirmTransfer", data);
    }

    [HttpPost]
    public async Task<IActionResult> TransferConfirmed([FromForm] TransferViewModel data)
    {
        int destNum = Int32.Parse(data.DestinationAccountNumber!);
        ITransferService.TransferError? result = await _transferService.Transfer(
            data.AccountNumber.GetValueOrDefault(),
            destNum,
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

    public IActionResult TransferCanceled()
    {
        return RedirectToAction(nameof(Index));
    }
}
