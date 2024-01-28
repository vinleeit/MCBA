using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Htmx;
using McbaAdmin.Models;
using McbaData.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace McbaAdmin.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _httpClient;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("api");
    }

    public IActionResult Index()
    {
        var token = GetToken();
        if (token == null)
        {
            return RedirectToAction(nameof(Login));
        }
        return RedirectToAction(nameof(Customers));
    }

    public IActionResult Login()
    {
        var token = GetToken();
        if (token != null)
        {
            return RedirectToAction(nameof(Customers));
        }
        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] LoginViewModel data)
    {
        if (!ModelState.IsValid)
        {
            return View(data);
        }
        HttpResponseMessage result = await _httpClient.PostAsync("api/Login", new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));
        if (result.StatusCode != System.Net.HttpStatusCode.OK)
        {
            ModelState.AddModelError("Username", "Invalid Username/Password");
            return View(data);
        }
        TokenJsonModel? token = await result.Content.ReadFromJsonAsync<TokenJsonModel>();
        HttpContext.Session.SetString("token", token!.Token);
        return RedirectToAction(nameof(Customers));
    }

    public async Task<IActionResult> Customers()
    {
        var token = GetToken();
        if (token == null)
        {
            return RedirectToAction(nameof(Login));
        }
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
        HttpResponseMessage result = await _httpClient.GetAsync("api/Customer");
        _ = result.EnsureSuccessStatusCode();
        List<CustomerDto>? customers = await result.Content.ReadFromJsonAsync<List<CustomerDto>>();
        return View(customers);
    }

    public async Task<IActionResult> EditCustomer([FromRoute] int id)
    {
        var token = GetToken();
        if (token == null)
        {
            return RedirectToAction(nameof(Login));
        }
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
        HttpResponseMessage result = await _httpClient.GetAsync($"api/Customer/{id}");
        _ = result.EnsureSuccessStatusCode();
        CustomerDto? customer = await result.Content.ReadFromJsonAsync<CustomerDto>();
        return customer == null ? NotFound() : View(customer);
    }

    [HttpPost]
    public async Task<IActionResult> EditCustomer([FromRoute] int id, CustomerDto data)
    {
        var token = GetToken();
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
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
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
        var token = GetToken();
        if (token == null)
        {
            return RedirectToAction(nameof(Login));
        }
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
        await _httpClient.PutAsync($"api/Lock/lock/{id}", null);
        if (HttpContext.Request.IsHtmx())
        {
            var result = await _httpClient.GetAsync($"api/Customer/{id}");
            _ = result.EnsureSuccessStatusCode();
            CustomerDto? c = await result.Content.ReadFromJsonAsync<CustomerDto>();
            return View("CustomerRowPartial", c!);
        }
        return RedirectToAction(nameof(Customers));
    }

    [HttpPost]
    public async Task<IActionResult> UnlockCustomer([FromRoute] int id)
    {
        var token = GetToken();
        if (token == null)
        {
            return RedirectToAction(nameof(Login));
        }
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
        _ = await _httpClient.PutAsync($"api/Lock/unlock/{id}", null);
        if (HttpContext.Request.IsHtmx())
        {
            var result = await _httpClient.GetAsync($"api/Customer/{id}");
            _ = result.EnsureSuccessStatusCode();
            CustomerDto? c = await result.Content.ReadFromJsonAsync<CustomerDto>();
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
