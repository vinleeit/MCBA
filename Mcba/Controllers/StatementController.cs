using Mcba.Middlewares;
using Mcba.Services.Interfaces;
using Mcba.ViewModels.Statement;
using Microsoft.AspNetCore.Mvc;

namespace Mcba.Controllers;

[LoggedIn]
public class StatementController(
    IAccountService accountService,
    IBalanceService balanceService,
    IStatementService statementService
) : Controller
{
    private readonly IAccountService _accountService = accountService;
    private readonly IStatementService _statementService = statementService;
    private readonly IBalanceService _balanceService = balanceService;

    public async Task<IActionResult> Index()
    {
        int customerID = HttpContext.Session.GetInt32("Customer")!.Value;
        var accounts = await _accountService.GetAccounts(customerID);
        return View(accounts);
    }

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
        return View(
            new StatementViewModel()
            {
                AccountNumber = Account,
                TotalBalance = await _balanceService.GetAccountBalance(Account),
                Page = Page,
                TotalPage = totalPage,
                Transactions = data.Select(e =>
                    {
                        var transactionTypeStr = "";
                        switch (e.TransactionType)
                        {
                            case ('D'):
                                transactionTypeStr = "Deposit";
                                break;
                            case ('W'):
                                transactionTypeStr = "Withdraw";
                                break;
                            case ('S'):
                                transactionTypeStr = "Service Charge";
                                break;
                            case ('T'):
                                if (e.AccountNumber == Account)
                                {
                                    transactionTypeStr = "Transfer (Debit)";
                                }
                                else
                                {
                                    transactionTypeStr = "Transfer (Credit)";
                                }
                                break;
                            case ('B'):
                                transactionTypeStr = "Bill Pay";
                                break;
                            default:
                                transactionTypeStr = "Undefined";
                                break;
                        }
                        return new TransactionViewModel()
                        {
                            TransactionID = e.TransactionID,
                            TransactionType = transactionTypeStr,
                            AccountNumber = e.AccountNumber,
                            DestinationAccountNumber = e.DestinationAccountNumber,
                            Amount = e.Amount,
                            Comment = e.Comment,
                            TransactionTimeLocal = e.TransactionTimeUtc.ToLocalTime(),
                        };
                    })
                    .ToList(),
            }
        );
    }
}
