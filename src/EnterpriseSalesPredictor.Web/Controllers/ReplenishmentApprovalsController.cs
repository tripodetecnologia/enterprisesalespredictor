using EnterpriseSalesPredictor.Web.Filters;
using EnterpriseSalesPredictor.Web.Services;
using EnterpriseSalesPredictor.Web.ViewModels.Replenishment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Web.Controllers;

[Authorize]
[RequirePermission("replenishment:read")]
public sealed class ReplenishmentApprovalsController : Controller
{
    private readonly ReplenishmentApiClient _replenishmentApiClient;

    public ReplenishmentApprovalsController(ReplenishmentApiClient replenishmentApiClient)
    {
        _replenishmentApiClient = replenishmentApiClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate, Guid? productId, int pageNumber = 1, CancellationToken cancellationToken = default)
    {
        var viewModel = await BuildPageModelAsync(fromDate, toDate, productId, pageNumber, cancellationToken);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("replenishment:write")]
    public async Task<IActionResult> Review(ReviewRecommendationFormViewModel model, DateTime? fromDate, DateTime? toDate, Guid? productId, int pageNumber = 1, CancellationToken cancellationToken = default)
    {
        try
        {
            await _replenishmentApiClient.ReviewRecommendationAsync(model.RecommendationId, model.Action, model.Notes, cancellationToken);
            TempData["StatusMessage"] = "La sugerencia se actualizó correctamente.";
            return RedirectToAction(nameof(Index), new { fromDate = fromDate?.ToString("yyyy-MM-dd"), toDate = toDate?.ToString("yyyy-MM-dd"), productId, pageNumber });
        }
        catch (Exception exception)
        {
            TempData["ErrorMessage"] = exception.Message;
            return RedirectToAction(nameof(Index), new { fromDate = fromDate?.ToString("yyyy-MM-dd"), toDate = toDate?.ToString("yyyy-MM-dd"), productId, pageNumber });
        }
    }

    private async Task<ReplenishmentApprovalPageViewModel> BuildPageModelAsync(DateTime? fromDate, DateTime? toDate, Guid? productId, int pageNumber, CancellationToken cancellationToken)
    {
        var options = await _replenishmentApiClient.GetOptionsAsync(cancellationToken);
        return new ReplenishmentApprovalPageViewModel
        {
            Recommendations = await _replenishmentApiClient.GetRecommendationsAsync("Pending", fromDate, toDate, productId, pageNumber, cancellationToken),
            FromDate = fromDate,
            ToDate = toDate,
            ProductId = productId,
            Products = options.Products,
            StatusMessage = TempData["StatusMessage"] as string
        };
    }
}
