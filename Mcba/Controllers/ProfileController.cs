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
        if (customer == null)
        {
            return RedirectToAction(nameof(Index));
        }
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

    [HttpPost]
    public async Task<IActionResult> Edit(ProfileViewModel edittedCustomer)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var error = await _profileService.UpdateCustomerProfile(new Models.Customer
        {
            CustomerID = edittedCustomer.CustomerID,
            Name = edittedCustomer.Name,
            TFN = edittedCustomer.TFN,
            Address = edittedCustomer.Address,
            City = edittedCustomer.City,
            State = edittedCustomer.State,
            Postcode = edittedCustomer.Postcode,
            Mobile = edittedCustomer.Mobile
        });

        if (error != null)
        {
            switch (error)
            {
                case (IProfileService.ProfileError.CustomerNotFound):
                    edittedCustomer.ErrorMsg = "No customer found!";
                    break;
                case (IProfileService.ProfileError.NoDataChange):
                    edittedCustomer.ErrorMsg = "No data modification found!";
                    break;
            }
            return View(edittedCustomer);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        await _profileService.UpdateCustomerPassword(HttpContext.Session.GetInt32("Customer") ?? -1, viewModel.Password);
        return RedirectToAction(nameof(Index));
    }

}