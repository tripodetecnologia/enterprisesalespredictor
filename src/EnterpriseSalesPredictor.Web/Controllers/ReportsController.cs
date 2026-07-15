using EnterpriseSalesPredictor.Web.Filters;
using EnterpriseSalesPredictor.Web.Services;
using EnterpriseSalesPredictor.Web.ViewModels.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Web.Controllers;

[Authorize]
[RequirePermission("reports:read")]
public sealed class ReportsController : Controller
{
    private readonly ReportsApiClient _reportsApiClient;

    public ReportsController(ReportsApiClient reportsApiClient)
    {
        _reportsApiClient = reportsApiClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] ReportFiltersViewModel filters, CancellationToken cancellationToken)
    {
        var viewModel = new ReportsPageViewModel
        {
            Filters = filters,
            ManagementReport = await _reportsApiClient.GetManagementReportAsync(filters, cancellationToken),
            CommercialReport = await _reportsApiClient.GetCommercialReportAsync(filters, cancellationToken),
            OperationalReport = await _reportsApiClient.GetOperationalReportAsync(filters, cancellationToken),
            ReplenishmentReport = await _reportsApiClient.GetReplenishmentReportAsync(filters, cancellationToken),
            PredictiveReport = await _reportsApiClient.GetPredictiveReportAsync(filters, cancellationToken)
        };

        return View(viewModel);
    }
}
