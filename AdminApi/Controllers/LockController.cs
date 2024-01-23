using AdminApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace AdminApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LockController(IAdminRepo adminRepo) : ControllerBase
{
    private readonly IAdminRepo _adminRepo = adminRepo;

    [HttpPut("lock/{customerID}")]
    public IActionResult Lock(int customerID)
    {
        return _adminRepo.GetCustomer(customerID) == null
            ? NotFound()
            : !_adminRepo.EditCustomerLock(customerID, true)
                ? StatusCode(500)
                : (IActionResult)Ok();
    }

    [HttpPut("unlock/{customerID}")]
    public IActionResult UnLock(int customerID)
    {
        return _adminRepo.GetCustomer(customerID) == null
            ? NotFound()
            : !_adminRepo.EditCustomerLock(customerID, false)
                ? StatusCode(500)
                : (IActionResult)Ok();
    }
}
