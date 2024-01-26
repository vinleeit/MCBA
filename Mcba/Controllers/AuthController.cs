using Mcba.Services.Interfaces;
using Mcba.ViewModels.Auth;
using Microsoft.AspNetCore.Mvc;
using static Mcba.Services.Interfaces.IAuthService;

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
        var result = await _authService.Login(loginData.LoginId, loginData.Password);
        if (result.Error.HasValue)
        {
            var errMsg = "";
            switch (result.Error)
            {
                case (AuthError.Locked):
                    errMsg = "Login is locked";
                    break;
                case (AuthError.InvalidCredential):
                    errMsg = "Incorrect Login ID or Password";
                    break;
            }
            ModelState.AddModelError("LoginId", errMsg);
            return View(loginData);
        }
        // Save auth info to session
        HttpContext.Session.SetInt32("Customer", result.Customer.GetValueOrDefault());
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
