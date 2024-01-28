using System.Diagnostics;
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
        return RedirectToAction(nameof(Login));
    }

    public IActionResult Login()
    {
        return View(new LoginViewModel()
        {
            Password = "ravel",
            Username = ""
        });
    }

    [HttpPost]
    public IActionResult Login([FromForm] LoginViewModel data)
    {
        if (!ModelState.IsValid)
        {
            return View(data);
        }
        return View(data);
    }

    public async Task<IActionResult> Customers()
    {
        HttpResponseMessage result = await _httpClient.GetAsync("api/Customer");
        _ = result.EnsureSuccessStatusCode();
        List<CustomerDto>? customers = await result.Content.ReadFromJsonAsync<List<CustomerDto>>();
        return View(customers);
    }

    public async Task<IActionResult> EditCustomer([FromRoute] int id)
    {
        HttpResponseMessage result = await _httpClient.GetAsync($"api/Customer/{id}");
        _ = result.EnsureSuccessStatusCode();
        CustomerDto? customer = await result.Content.ReadFromJsonAsync<CustomerDto>();
        return customer == null ? NotFound() : View(customer);
    }

    [HttpPost]
    public async Task<IActionResult> EditCustomer([FromRoute] int id, CustomerDto data)
    {
        if (!ModelState.IsValid)
        {
            data.CustomerId = id;
            return View(data);
        }
        string dataJson = JsonSerializer.Serialize(data);
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
}
