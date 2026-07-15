using EnterpriseSalesPredictor.Api.Contracts.Forecasting;
using EnterpriseSalesPredictor.Application.Interfaces.Forecasting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Api.Controllers;

[ApiController]
[Route("api/forecasts")]
[Authorize]
[Authorize(Policy = "Permission:forecasts:write")]
public sealed class ForecastsController : ControllerBase
{
    private readonly IForecastService _forecastService;

    public ForecastsController(IForecastService forecastService)
    {
        _forecastService = forecastService;
    }

    [HttpPost]
    public async Task<IActionResult> GenerateAsync([FromBody] GenerateForecastRequest request, CancellationToken cancellationToken)
    {
        var result = await _forecastService.GenerateForecastAsync(new ForecastQuery
        {
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            ProductId = request.ProductId,
            CustomerId = request.CustomerId,
            RequestedBy = User.Identity?.Name ?? "system"
        }, cancellationToken);

        return Ok(result);
    }
}
