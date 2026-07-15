using EnterpriseSalesPredictor.Web.Filters;
using EnterpriseSalesPredictor.Web.Services;
using EnterpriseSalesPredictor.Web.ViewModels.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Web.Controllers;

[Authorize]
[RequirePermission("sales:read")]
public sealed class SalesController : Controller
{
    private readonly SalesApiClient _salesApiClient;

    public SalesController(SalesApiClient salesApiClient)
    {
        _salesApiClient = salesApiClient;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new SalesQueryPageViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> Query([FromQuery] SalesQueryFilterViewModel filters, CancellationToken cancellationToken)
    {
        var results = await _salesApiClient.QuerySalesAsync(filters, cancellationToken);
        return Json(results);
    }
}
