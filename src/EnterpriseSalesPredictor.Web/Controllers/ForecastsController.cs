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

    [HttpGet]
    public IActionResult Index()
    {
        return View(new ForecastPageViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Generate([FromBody] ForecastRequestViewModel request, CancellationToken cancellationToken)
    {
        var result = await _forecastsApiClient.GenerateForecastAsync(request, cancellationToken);
        return Json(result);
    }
}
