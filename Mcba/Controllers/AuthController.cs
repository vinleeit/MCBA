using Mcba.Services.Interfaces;
using Mcba.ViewModels.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Mcba.Controllers;

public class AuthController(IAuthService authService) : Controller
{
    private readonly IAuthService _authService = authService;

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] LoginViewModel loginData)
    {
        if (!ModelState.IsValid)
        {
            return View(loginData);
        }
        var customer = await _authService.Login(loginData.LoginId, loginData.Password);
        if (customer == null)
        {
            ModelState.AddModelError("LoginId", "Incorrect Login ID or Password");
            return View(loginData);
        }
        // Save auth info to session
        HttpContext.Session.SetString("Customer", customer.ToString()!);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
