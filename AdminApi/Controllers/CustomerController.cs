using AdminApi.Data;
using McbaData.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController(IAdminRepo adminRepo) : ControllerBase
{
    // Use injected Admin Repository
    private readonly IAdminRepo _adminRepo = adminRepo;

    [Authorize]
    [HttpGet]
    public List<CustomerDto> GetAll()
    {
        return _adminRepo.GetCustomers();
    }

    [Authorize]
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        CustomerDto? customer = _adminRepo.GetCustomer(id);
        return customer == null ? NotFound() : Ok(customer);
    }

    [Authorize]
    [HttpGet("exists/{id}")]
    public IActionResult GetAccountExists(int id)
    {
        return _adminRepo.GetCustomer(id) != null ? Ok() : NotFound();
    }

    [Authorize]
    [HttpPost("{id}")]
    public IActionResult EditCustomer(int id, [FromBody] CustomerEditDto customer)
    {
        return _adminRepo.GetCustomer(id) == null
            ? NotFound()
            : _adminRepo.EditCustomer(customer, id)
                ? Ok()
                // Return internal server error if the edit is not successful
                : (IActionResult)StatusCode(500);
    }
}
