using System.Diagnostics;
using EnterpriseSalesPredictor.Web.Filters;
using EnterpriseSalesPredictor.Web.Models;
using EnterpriseSalesPredictor.Web.Services;
using EnterpriseSalesPredictor.Web.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DashboardApiClient _dashboardApiClient;

    public HomeController(ILogger<HomeController> logger, DashboardApiClient dashboardApiClient)
    {
        _logger = logger;
        _dashboardApiClient = dashboardApiClient;
    }

    [RequirePermission("dashboard:read")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var viewModel = new DashboardPageViewModel
        {
            Kpis = await _dashboardApiClient.GetKpisAsync(cancellationToken),
            TopCustomers = await _dashboardApiClient.GetTopCustomersAsync(cancellationToken),
            TopProducts = await _dashboardApiClient.GetTopProductsAsync(cancellationToken),
            SalesByLine = await _dashboardApiClient.GetSalesByLineAsync(cancellationToken),
            SalesBySupplier = await _dashboardApiClient.GetSalesBySupplierAsync(cancellationToken),
            Alerts = await _dashboardApiClient.GetAlertsAsync(cancellationToken)
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [AllowAnonymous]
    public IActionResult Forbidden()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult NotFoundPage()
    {
        return View();
    }
}
