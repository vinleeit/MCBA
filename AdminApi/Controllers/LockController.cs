using Microsoft.AspNetCore.Mvc;

namespace AdminApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LockController : ControllerBase
{
    [HttpPut("lock/{customerID}")]
    public IActionResult Lock(int customerID)
    {
        return Ok();
    }

    [HttpPut("unlock/{customerID}")]
    public IActionResult UnLock(int customerID)
    {
        return Ok();
    }
}
