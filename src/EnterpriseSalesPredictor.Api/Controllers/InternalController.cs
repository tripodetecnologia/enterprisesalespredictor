using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Api.Controllers;

[ApiController]
[Route("api/internal")]
[Authorize(Policy = PermissionPolicies.SystemRead)]
public sealed class InternalController : ControllerBase
{
    [HttpGet("status")]
    public IActionResult Status()
    {
        return Ok(new
        {
            status = "Secure endpoint available"
        });
    }
}
