using Mcba.Data;
using Mcba.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace Mcba.Controllers;

[LoggedIn]
public class ProfileController(McbaContext context) : Controller
{
    private readonly McbaContext _dbContext = context;


    public IActionResult Index()
    {
        var customerID = HttpContext.Session.GetInt32("Customer");
        var customer = _dbContext.Customers.FirstOrDefault(b => b.CustomerID == customerID);
        return View(customer);
    }
}