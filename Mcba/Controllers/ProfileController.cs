using Mcba.Data;
using Mcba.Middlewares;
using Mcba.Models;
using Mcba.Services;
using Mcba.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Mcba.Controllers;

[LoggedIn]
public class ProfileController(McbaContext context, IProfileService profileService) : Controller
{
    private readonly McbaContext _dbContext = context;
    private readonly IProfileService _profileService = profileService;


    public IActionResult Index()
    {
        var customerID = HttpContext.Session.GetInt32("Customer");
        var customer = _dbContext.Customers.FirstOrDefault(b => b.CustomerID == customerID);
        return View(customer);
    }

    [HttpGet]
    public IActionResult Edit() {
        var customerID = HttpContext.Session.GetInt32("Customer");
        var customer = _dbContext.Customers.FirstOrDefault(b => b.CustomerID == customerID);
        return View(customer);
    }

    [HttpPost]
    public IActionResult Edit(Customer customer) {
        if (!ModelState.IsValid) {
            return View();
        }
        // TODO: Redirectt to confirmation view
        return RedirectToAction(nameof(Index));
    }
}