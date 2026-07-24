using EnterpriseSalesPredictor.Application.Interfaces.Auditing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Api.Controllers;

[ApiController]
[Route("api/audit")]
[Authorize]
public sealed class AuditController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet]
    [Authorize(Policy = PermissionPolicies.AuditRead)]
    public async Task<IActionResult> GetAuditLogsAsync(CancellationToken cancellationToken)
    {
        var logs = await _auditLogService.GetAuditLogsAsync(cancellationToken);
        return Ok(logs);
    }

}
