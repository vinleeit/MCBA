using Mcba.Data;
using Mcba.Middlewares;
using Mcba.ViewModels.Profile;
using Mcba.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    public IActionResult Edit()
    {
        var customerID = HttpContext.Session.GetInt32("Customer");
        var customer = _dbContext.Customers.FirstOrDefault(b => b.CustomerID == customerID);
        if (customer != null)
        {
            return View(new ProfileViewModel()
            {
                CustomerID = customer.CustomerID,
                Name = customer.Name,
                TFN = customer.TFN,
                Address = customer.TFN,
                City = customer.City,
                State = customer.State,
                Postcode = customer.Postcode,
                Mobile = customer.Mobile
            });
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Edit(ProfileViewModel edittedCustomer)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        // TODO: Redirectt to confirmation view
        return RedirectToAction(nameof(Index));
    }
}