using EnterpriseSalesPredictor.Web.Filters;
using EnterpriseSalesPredictor.Web.Services;
using EnterpriseSalesPredictor.Web.ViewModels.Replenishment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Web.Controllers;

[Authorize]
public sealed class ReplenishmentController : Controller
{
    private readonly ReplenishmentApiClient _replenishmentApiClient;

    public ReplenishmentController(ReplenishmentApiClient replenishmentApiClient)
    {
        _replenishmentApiClient = replenishmentApiClient;
    }

    [HttpGet]
    [RequirePermission("replenishment:read")]
    public async Task<IActionResult> Index([FromQuery] ReplenishmentProjectionFilterViewModel filters, int pageNumber = 1, CancellationToken cancellationToken = default)
    {
        if (filters.FromDate.HasValue && filters.ToDate.HasValue && string.IsNullOrWhiteSpace(filters.ProductName))
        {
            ModelState.AddModelError(nameof(filters.ProductName), "El producto es obligatorio.");
        }

        var viewModel = await BuildPageModelAsync(filters, pageNumber, cancellationToken);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("replenishment:write")]
    public async Task<IActionResult> Submit(ReplenishmentProjectionViewModel model, ReplenishmentProjectionFilterViewModel filters, int pageNumber = 1, CancellationToken cancellationToken = default)
    {
        try
        {
            await _replenishmentApiClient.SubmitProjectionAsync(model, cancellationToken);
            TempData["StatusMessage"] = "Sugerencia enviada a aprobación correctamente.";
            return RedirectToAction(nameof(Index), new
            {
                fromDate = filters.FromDate?.ToString("yyyy-MM-dd"),
                toDate = filters.ToDate?.ToString("yyyy-MM-dd"),
                customerId = filters.CustomerId,
                productName = filters.ProductName,
                pageNumber
            });
        }
        catch (Exception exception)
        {
            var errorModel = await BuildPageModelAsync(filters, pageNumber, cancellationToken);
            errorModel.ErrorMessage = exception.Message;
            return View("Index", errorModel);
        }
    }

    private async Task<ReplenishmentProjectionPageViewModel> BuildPageModelAsync(ReplenishmentProjectionFilterViewModel filters, int pageNumber, CancellationToken cancellationToken)
    {
        var options = await _replenishmentApiClient.GetOptionsAsync(cancellationToken);
        var results = (!filters.FromDate.HasValue || !filters.ToDate.HasValue)
            ? new PagedReplenishmentProjectionResultViewModel()
            : await _replenishmentApiClient.GetProjectionsAsync(filters, pageNumber, cancellationToken);

        return new ReplenishmentProjectionPageViewModel
        {
            Filters = filters,
            Results = results,
            Customers = options.Customers,
            Products = options.Products
                .GroupBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                .Select(group => new ReplenishmentProductOptionViewModel { Name = group.Key })
                .OrderBy(item => item.Name)
                .ToArray(),
            StatusMessage = TempData["StatusMessage"] as string
        };
    }
}
