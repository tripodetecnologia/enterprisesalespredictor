using EnterpriseSalesPredictor.Web.Filters;
using EnterpriseSalesPredictor.Web.Services;
using EnterpriseSalesPredictor.Web.ViewModels.Forecasting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Web.Controllers;

[Authorize]
[RequirePermission("forecasts:write")]
public sealed class ForecastsController : Controller
{
    private readonly ForecastsApiClient _forecastsApiClient;

    public ForecastsController(ForecastsApiClient forecastsApiClient)
    {
        _forecastsApiClient = forecastsApiClient;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var viewModel = await _forecastsApiClient.GetOptionsAsync(cancellationToken);
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Generate([FromBody] ForecastRequestViewModel request, CancellationToken cancellationToken)
    {
        if (!request.FromDate.HasValue)
        {
            return BadRequest("La fecha de inicio es obligatoria.");
        }

        if (!request.ToDate.HasValue)
        {
            return BadRequest("La fecha de fin es obligatoria.");
        }

        var result = await _forecastsApiClient.GenerateForecastAsync(request, cancellationToken);
        return Json(result);
    }
}
