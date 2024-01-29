using Mcba.Middlewares;
using Mcba.Services.Interfaces;
using Mcba.ViewModels.Profile;
using McbaData;
using Microsoft.AspNetCore.Mvc;

namespace Mcba.Controllers;

[LoggedIn]
public class ProfileController(McbaContext context, IProfileService profileService) : Controller
{
    private readonly McbaContext _dbContext = context;
    private readonly IProfileService _profileService = profileService;

    public IActionResult Index()
    {
        McbaData.Models.Customer? customer = _dbContext.Customers.FirstOrDefault(
            b => b.CustomerID == HttpContext.Session.GetInt32("Customer")
        );
        return customer == null ? NotFound() : View(customer);
    }

    [HttpGet]
    public IActionResult Edit()
    {
        McbaData.Models.Customer? customer = _dbContext.Customers.FirstOrDefault(
            b => b.CustomerID == HttpContext.Session.GetInt32("Customer")
        );
        return customer != null
            ? View(
                new ProfileViewModel()
                {
                    CustomerID = customer.CustomerID,
                    Name = customer.Name,
                    TFN = customer.TFN,
                    Address = customer.TFN,
                    City = customer.City,
                    State = customer.State,
                    Postcode = customer.Postcode,
                    Mobile = customer.Mobile
                }
            )
            : RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProfileViewModel edittedCustomer)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        IProfileService.ProfileError? error = await _profileService.UpdateCustomerProfile(
            new McbaData.Models.Customer
            {
                CustomerID = edittedCustomer.CustomerID,
                Name = edittedCustomer.Name!,
                TFN = edittedCustomer.TFN,
                Address = edittedCustomer.Address,
                City = edittedCustomer.City,
                State = edittedCustomer.State,
                Postcode = edittedCustomer.Postcode,
                Mobile = edittedCustomer.Mobile
            }
        );
        if (error is not null and IProfileService.ProfileError.NoDataChange)
        {
            TempData["Error"] = "No data modification found!";
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
        _ = await _profileService.UpdateCustomerPassword(
            HttpContext.Session.GetInt32("Customer") ?? -1,
            viewModel.Password
        );
        return RedirectToAction(nameof(Index));
    }
}
