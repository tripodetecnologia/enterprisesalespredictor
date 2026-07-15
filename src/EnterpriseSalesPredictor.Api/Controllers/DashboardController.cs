using EnterpriseSalesPredictor.Application.Interfaces.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
[Authorize(Policy = "Permission:dashboard:read")]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardReadService _dashboardReadService;

    public DashboardController(IDashboardReadService dashboardReadService)
    {
        _dashboardReadService = dashboardReadService;
    }

    [HttpGet("kpis")]
    public async Task<IActionResult> GetKpisAsync([FromQuery] DashboardQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _dashboardReadService.GetKpisAsync(criteria, cancellationToken);
        return Ok(result);
    }

    [HttpGet("top-customers")]
    public async Task<IActionResult> GetTopCustomersAsync([FromQuery] DashboardQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _dashboardReadService.GetTopCustomersAsync(criteria, cancellationToken);
        return Ok(result);
    }

    [HttpGet("top-products")]
    public async Task<IActionResult> GetTopProductsAsync([FromQuery] DashboardQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _dashboardReadService.GetTopProductsAsync(criteria, cancellationToken);
        return Ok(result);
    }

    [HttpGet("sales-by-line")]
    public async Task<IActionResult> GetSalesByLineAsync([FromQuery] DashboardQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _dashboardReadService.GetSalesByProductLineAsync(criteria, cancellationToken);
        return Ok(result);
    }

    [HttpGet("sales-by-supplier")]
    public async Task<IActionResult> GetSalesBySupplierAsync([FromQuery] DashboardQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _dashboardReadService.GetSalesBySupplierAsync(criteria, cancellationToken);
        return Ok(result);
    }

    [HttpGet("alerts")]
    public async Task<IActionResult> GetAlertsAsync([FromQuery] DashboardQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _dashboardReadService.GetCommercialAlertsAsync(criteria, cancellationToken);
        return Ok(result);
    }
}
