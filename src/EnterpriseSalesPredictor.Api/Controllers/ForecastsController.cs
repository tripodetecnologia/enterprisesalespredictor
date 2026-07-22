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

    [HttpGet("options")]
    public async Task<IActionResult> GetOptionsAsync(CancellationToken cancellationToken)
    {
        var result = await _forecastService.GetOptionsAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> GenerateAsync([FromBody] GenerateForecastRequest request, CancellationToken cancellationToken)
    {
        if (!request.FromDate.HasValue)
        {
            return BadRequest(new { message = "La fecha de inicio es obligatoria." });
        }

        if (!request.ToDate.HasValue)
        {
            return BadRequest(new { message = "La fecha de fin es obligatoria." });
        }

        var result = await _forecastService.GenerateForecastAsync(new ForecastQuery
        {
            FromDate = request.FromDate.Value,
            ToDate = request.ToDate.Value,
            ProductName = request.ProductName,
            CustomerId = request.CustomerId,
            RequestedBy = User.Identity?.Name ?? "system"
        }, cancellationToken);

        return Ok(result);
    }
}
