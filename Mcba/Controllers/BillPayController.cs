using System.Text.Json;
using Mcba.Data;
using Mcba.Middlewares;
using Mcba.Models;
using Mcba.Services.Interfaces;
using Mcba.ViewModels.BillPay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mcba.Controllers;

[LoggedIn]
public class BillPayController(McbaContext dbContext, IAccountService accountService, IBillPayService billPayService) : Controller
{
    private readonly McbaContext _dbContext = dbContext;
    private readonly IAccountService _accountService = accountService;
    private readonly IBillPayService _billPayService = billPayService;

    public async Task<IActionResult> Index()
    {
        var billPays = await _billPayService.GetBillPays();
        return View(billPays);
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var customerID = HttpContext.Session.GetInt32("Customer");
        var accounts = await _accountService.GetAccounts(customerID.GetValueOrDefault());
        var serialized = JsonSerializer.SerializeToUtf8Bytes(accounts);
        HttpContext.Session.Set("accounts", serialized);
        return View(new BillPayViewModel()
        {
            Accounts = accounts,
        });
    }

    [HttpPost]
    public async Task<IActionResult> Add(BillPayViewModel newBillPayViewModel)
    {
        var payee = await _dbContext.Payees.FirstOrDefaultAsync(b => b.PayeeID == newBillPayViewModel.PayeeID);
        if (payee == null)
        {
            ModelState.AddModelError("PayeeID", "Payee does not exist");
        }

        var localDT = DateTime.Now;
        localDT = new DateTime(localDT.Year, localDT.Month, localDT.Day, localDT.Hour, localDT.Minute, 0);
        newBillPayViewModel.ScheduleTimeLocal = new DateTime(newBillPayViewModel.ScheduleTimeLocal.Year, newBillPayViewModel.ScheduleTimeLocal.Month, newBillPayViewModel.ScheduleTimeLocal.Day, newBillPayViewModel.ScheduleTimeLocal.Hour, newBillPayViewModel.ScheduleTimeLocal.Minute, 0);
        if (localDT.CompareTo(newBillPayViewModel.ScheduleTimeLocal) > 0)
        {
            ModelState.AddModelError("ScheduleTimeLocal", "Date has overdue");
        }

        if (!ModelState.IsValid)
        {
            var serializedList = HttpContext.Session.Get("accounts");
            var accounts = JsonSerializer.Deserialize<List<Account>>(serializedList);
            newBillPayViewModel.Accounts = accounts ?? [];
            return View(newBillPayViewModel);
        }

        await _billPayService.AddBillPay(newBillPayViewModel);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Cancel(int id)
    {
        var billPay = await _dbContext.BillPays.FirstOrDefaultAsync(b => b.BillPayID == id);
        if (billPay != null)
        {
            var localDT = DateTime.Now;
            localDT = new DateTime(localDT.Year, localDT.Month, localDT.Day, localDT.Hour, localDT.Minute, 0);
            if (billPay.ScheduleTimeUtc < localDT)
            {
                await _billPayService.PayBillPay(id, true);
            }
            else
            {
                await _billPayService.DeleteBillPay(id);
            }
        }
        return RedirectToAction(nameof(Index));
    }
}