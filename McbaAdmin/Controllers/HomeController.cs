using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Htmx;
using McbaAdmin.Models;
using McbaData.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace McbaAdmin.Controllers;

public class HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("api");

    public IActionResult Index()
    {
        // If there is no token (not logged in go to login otherwise go to list of customer)
        string? token = GetToken();
        return token == null
            ? RedirectToAction(nameof(Login))
            : (IActionResult)RedirectToAction(nameof(Customers));
    }

    public IActionResult Login()
    {
        string? token = GetToken();
        return token != null ? RedirectToAction(nameof(Customers)) : View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] LoginViewModel data)
    {
        if (!ModelState.IsValid)
        {
            return View(data);
        }
        // Send post request to api with json body
        HttpResponseMessage result = await _httpClient.PostAsync(
            "api/Login",
            new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
        );
        // If the system does not authorize login
        if (result.StatusCode != System.Net.HttpStatusCode.OK)
        {
            ModelState.AddModelError("Username", "Invalid Username/Password");
            return View(data);
        }
        // Get JWT and store it in the session cookies
        TokenJsonModel? token = await result.Content.ReadFromJsonAsync<TokenJsonModel>();
        HttpContext.Session.SetString("token", token!.Token);
        return RedirectToAction(nameof(Customers));
    }

    public async Task<IActionResult> Customers()
    {
        string? token = GetToken();
        if (token == null)
        {
            return RedirectToAction(nameof(Login));
        }
        // Add JWT to request header
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            HttpContext.Session.GetString("token")
        );
        HttpResponseMessage result = await _httpClient.GetAsync("api/Customer");
        _ = result.EnsureSuccessStatusCode();
        List<CustomerDto>? customers = await result.Content.ReadFromJsonAsync<List<CustomerDto>>();
        return View(customers);
    }

    public async Task<IActionResult> EditCustomer([FromRoute] int id)
    {
        string? token = GetToken();
        if (token == null)
        {
            return RedirectToAction(nameof(Login));
        }
        // Add JWT to request header
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            HttpContext.Session.GetString("token")
        );
        HttpResponseMessage result = await _httpClient.GetAsync($"api/Customer/{id}");
        _ = result.EnsureSuccessStatusCode();
        CustomerDto? customer = await result.Content.ReadFromJsonAsync<CustomerDto>();
        return customer == null ? NotFound() : View(customer);
    }

    [HttpPost]
    public async Task<IActionResult> EditCustomer([FromRoute] int id, CustomerDto data)
    {
        string? token = GetToken();
        if (token == null)
        {
            return RedirectToAction(nameof(Login));
        }
        if (!ModelState.IsValid)
        {
            data.CustomerId = id;
            return View(data);
        }
        string dataJson = JsonSerializer.Serialize(data);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            HttpContext.Session.GetString("token")
        );
        HttpResponseMessage result = await _httpClient.PostAsync(
            $"api/Customer/{id}",
            new StringContent(dataJson, Encoding.UTF8, "application/json")
        );
        _ = result.EnsureSuccessStatusCode();
        return RedirectToAction(nameof(Customers));
    }

    [HttpPost]
    public async Task<IActionResult> LockCustomer([FromRoute] int id)
    {
        string? token = GetToken();
        if (token == null)
        {
            return RedirectToAction(nameof(Login));
        }
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            HttpContext.Session.GetString("token")
        );
        await _httpClient.PutAsync($"api/Lock/lock/{id}", null);
        // Check if the result requested by HTMX
        if (HttpContext.Request.IsHtmx())
        {
            var result = await _httpClient.GetAsync($"api/Customer/{id}");
            _ = result.EnsureSuccessStatusCode();
            CustomerDto? c = await result.Content.ReadFromJsonAsync<CustomerDto>();
            // Return partials of the button to be swapped with morphdom
            return View("CustomerRowPartial", c!);
        }
        return RedirectToAction(nameof(Customers));
    }

    [HttpPost]
    public async Task<IActionResult> UnlockCustomer([FromRoute] int id)
    {
        string? token = GetToken();
        if (token == null)
        {
            return RedirectToAction(nameof(Login));
        }
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            HttpContext.Session.GetString("token")
        );
        _ = await _httpClient.PutAsync($"api/Lock/unlock/{id}", null);
        if (HttpContext.Request.IsHtmx())
        {
            HttpResponseMessage result = await _httpClient.GetAsync($"api/Customer/{id}");
            _ = result.EnsureSuccessStatusCode();
            CustomerDto? c = await result.Content.ReadFromJsonAsync<CustomerDto>();
            // Return partials of the button to be swapped with morphdom
            return View("CustomerRowPartial", c!);
        }
        return RedirectToAction(nameof(Customers));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Index));
    }

    // Utils function
    private string? GetToken()
    {
        return HttpContext.Session.GetString("token");
    }
}
