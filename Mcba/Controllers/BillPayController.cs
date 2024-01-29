using System.Text.Json;
using Mcba.Middlewares;
using Mcba.Services.Interfaces;
using Mcba.ViewModels.BillPay;
using McbaData;
using McbaData.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Mcba.Services.Interfaces.IBillPayService;

namespace Mcba.Controllers;

[LoggedIn]
public class BillPayController(
    McbaContext dbContext,
    IAccountService accountService,
    IBillPayService billPayService
) : Controller
{
    private readonly McbaContext _dbContext = dbContext;
    private readonly IAccountService _accountService = accountService;
    private readonly IBillPayService _billPayService = billPayService;

    public async Task<IActionResult> Index()
    {
        List<BillPayViewModel> billPays = await _billPayService.GetBillPays();
        return View(billPays);
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        // Get accounts of customer stored in the session and store accounts to session
        int? customerID = HttpContext.Session.GetInt32("Customer");
        List<Account> accounts = await _accountService.GetAccounts(customerID.GetValueOrDefault());
        byte[] serialized = JsonSerializer.SerializeToUtf8Bytes(accounts);
        HttpContext.Session.Set("accounts", serialized);

        DateTime dt = DateTime.Now;
        return View(
            new BillPayViewModel()
            {
                ScheduleTimeLocal = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0),
                Accounts = accounts,
            }
        );
    }

    [HttpPost]
    public async Task<IActionResult> Add(BillPayViewModel newBillPayViewModel)
    {
        Payee? payee = await _dbContext.Payees.FirstOrDefaultAsync(
            b => b.PayeeID == newBillPayViewModel.PayeeID
        );
        if (payee == null)
        {
            ModelState.AddModelError("PayeeID", "Payee ID is not registered");
        }

        DateTime localDT = DateTime.Now;
        localDT = new DateTime(
            localDT.Year,
            localDT.Month,
            localDT.Day,
            localDT.Hour,
            localDT.Minute,
            0
        );
        newBillPayViewModel.ScheduleTimeLocal = new DateTime(
            newBillPayViewModel.ScheduleTimeLocal.Year,
            newBillPayViewModel.ScheduleTimeLocal.Month,
            newBillPayViewModel.ScheduleTimeLocal.Day,
            newBillPayViewModel.ScheduleTimeLocal.Hour,
            newBillPayViewModel.ScheduleTimeLocal.Minute,
            0
        );
        if (localDT.CompareTo(newBillPayViewModel.ScheduleTimeLocal) > 0)
        {
            ModelState.AddModelError("ScheduleTimeLocal", "Date/Time has passed");
        }

        if (!ModelState.IsValid)
        {
            byte[]? serializedList = HttpContext.Session.Get("accounts");
            List<Account>? accounts = JsonSerializer.Deserialize<List<Account>>(serializedList);
            newBillPayViewModel.Accounts = accounts ?? [];
            return View(newBillPayViewModel);
        }

        await _billPayService.AddBillPay(newBillPayViewModel);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Action(int id)
    {
        BillPay? billPay = await _dbContext.BillPays.FirstOrDefaultAsync(b => b.BillPayID == id);
        if (billPay != null)
        {
            DateTime localDT = DateTime.Now;
            localDT = new DateTime(
                localDT.Year,
                localDT.Month,
                localDT.Day,
                localDT.Hour,
                localDT.Minute,
                0
            );
            if (billPay.ScheduleTimeUtc.ToLocalTime() < localDT)
            {
                BillPayError? err = await _billPayService.RetryBillPay(id);
                switch (err)
                {
                    case BillPayError.InsuffientBalance:
                        TempData["ActionError"] = "Insufficient balance to perform retry";
                        break;
                    case BillPayError.NotExist:
                        TempData["ActionError"] = "Operation failed, BillPay does not exist";
                        break;
                }
            }
            else
            {
                await _billPayService.DeleteBillPay(id);
            }
        }
        else
        {
            // Canceling / Paying but the BillPay is no longer exist, then show error
            TempData["ActionError"] = "Operation failed, BillPay does not exist";
        }
        return RedirectToAction(nameof(Index));
    }
}

