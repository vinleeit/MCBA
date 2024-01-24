using Mcba.Data;
using Mcba.Middlewares;
using Mcba.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Mcba.Controllers;

[LoggedIn]
public class BillPayController(McbaContext dbContext, IBillPayService billPayService) : Controller
{
    private readonly McbaContext _dbContext = dbContext;
    private readonly IBillPayService _billPayService = billPayService;

    public async Task<IActionResult> Index()
    {
        var billPays = await _billPayService.GetBillPays();
        return View(billPays);
    }
    
    public async Task<IActionResult> Cancel(int id) {
        await _billPayService.DeleteBillPay(id);
        return RedirectToAction(nameof(Index));
    }
}