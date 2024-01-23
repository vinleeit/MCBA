using Mcba.Middlewares;
using Mcba.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Mcba.Controllers;

public class StatementController(IAccountService accountService, IStatementService statementService)
    : Controller
{
    private readonly IAccountService _accountService = accountService;
    private readonly IStatementService _statementService = statementService;

    [LoggedIn]
    public async Task<IActionResult> Index()
    {
        int customerID = HttpContext.Session.GetInt32("Customer")!.Value;
        var accounts = await _accountService.GetAccounts(customerID);
        return View(accounts);
    }

    [LoggedIn]
    public async Task<IActionResult> Show([FromQuery] int Account, [FromQuery] int Page = 1)
    {
        var (totalPage, data) = await _statementService.GetPaginatedAccountTransactions(
            Account,
            (Page, 4)
        );
        if (Page > totalPage)
        {
            return NotFound();
        }
        ViewBag.TotalPage = totalPage;
        ViewBag.Page = Page;
        ViewBag.Account = Account;
        return View(data);
    }
}

