using EnterpriseSalesPredictor.Application.Interfaces.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Api.Controllers;

[ApiController]
[Route("api/sales")]
[Authorize]
[Authorize(Policy = "Permission:sales:read")]
public sealed class SalesController : ControllerBase
{
    private readonly ISalesReadService _salesReadService;

    public SalesController(ISalesReadService salesReadService)
    {
        _salesReadService = salesReadService;
    }

    [HttpGet("range")]
    public async Task<IActionResult> GetByRangeAsync([FromQuery] SalesQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _salesReadService.QuerySalesAsync(criteria, cancellationToken);
        return Ok(result);
    }

    [HttpGet("by-customer")]
    public async Task<IActionResult> GetByCustomerAsync([FromQuery] SalesQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _salesReadService.GetSalesByCustomerAsync(criteria, cancellationToken);
        return Ok(result);
    }

    [HttpGet("by-product")]
    public async Task<IActionResult> GetByProductAsync([FromQuery] SalesQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _salesReadService.GetSalesByProductAsync(criteria, cancellationToken);
        return Ok(result);
    }

    [HttpGet("by-supplier")]
    public async Task<IActionResult> GetBySupplierAsync([FromQuery] SalesQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _salesReadService.GetSalesBySupplierAsync(criteria, cancellationToken);
        return Ok(result);
    }

    [HttpGet("by-seller")]
    public async Task<IActionResult> GetBySellerAsync([FromQuery] SalesQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _salesReadService.GetSalesBySellerAsync(criteria, cancellationToken);
        return Ok(result);
    }

    [HttpGet("by-location")]
    public async Task<IActionResult> GetByLocationAsync([FromQuery] SalesQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _salesReadService.GetSalesByLocationAsync(criteria, cancellationToken);
        return Ok(result);
    }

    [HttpGet("comparisons/{period}")]
    public async Task<IActionResult> GetComparisonsAsync(string period, [FromQuery] SalesQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _salesReadService.GetSalesComparisonsAsync(period, criteria, cancellationToken);
        return Ok(result);
    }
}
