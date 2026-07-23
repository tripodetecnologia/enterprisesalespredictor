using EnterpriseSalesPredictor.Application.Interfaces.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Api.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
[Authorize(Policy = PermissionPolicies.ReportsRead)]
public sealed class ReportsController : ControllerBase
{
    private readonly IReportReadService _reportReadService;

    public ReportsController(IReportReadService reportReadService)
    {
        _reportReadService = reportReadService;
    }

    [HttpGet("management")]
    public async Task<IActionResult> GetManagementReportAsync([FromQuery] ReportQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _reportReadService.GetManagementReportAsync(criteria, cancellationToken);
        return Ok(result);
    }

    [HttpGet("commercial")]
    public async Task<IActionResult> GetCommercialReportAsync([FromQuery] ReportQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _reportReadService.GetCommercialReportAsync(criteria, cancellationToken);
        return Ok(result);
    }

    [HttpGet("operational")]
    public async Task<IActionResult> GetOperationalReportAsync([FromQuery] ReportQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _reportReadService.GetOperationalReportAsync(criteria, cancellationToken);
        return Ok(result);
    }

    [HttpGet("replenishment")]
    public async Task<IActionResult> GetReplenishmentReportAsync([FromQuery] ReportQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _reportReadService.GetReplenishmentReportAsync(criteria, cancellationToken);
        return Ok(result);
    }

    [HttpGet("predictive")]
    public async Task<IActionResult> GetPredictiveReportAsync([FromQuery] ReportQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _reportReadService.GetPredictiveReportAsync(criteria, cancellationToken);
        return Ok(result);
    }
}
