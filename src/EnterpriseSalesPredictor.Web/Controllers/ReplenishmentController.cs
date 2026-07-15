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
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var viewModel = await BuildPageModelAsync(cancellationToken);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("replenishment:write")]
    public async Task<IActionResult> Generate(GenerateRecommendationFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || !model.ProductId.HasValue)
        {
            var invalidModel = await BuildPageModelAsync(cancellationToken);
            invalidModel.GenerateForm = model;
            invalidModel.ErrorMessage = "Please provide a valid product id.";
            return View("Index", invalidModel);
        }

        try
        {
            await _replenishmentApiClient.GenerateRecommendationAsync(model.ProductId.Value, cancellationToken);
            TempData["StatusMessage"] = "Recommendation generated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception)
        {
            var errorModel = await BuildPageModelAsync(cancellationToken);
            errorModel.GenerateForm = model;
            errorModel.ErrorMessage = exception.Message;
            return View("Index", errorModel);
        }
    }

    [HttpGet]
    [RequirePermission("replenishment:read")]
    public async Task<IActionResult> Detail(Guid id, CancellationToken cancellationToken)
    {
        var recommendation = await GetRecommendationAsync(id, cancellationToken);
        if (recommendation is null)
        {
            return RedirectToAction("NotFoundPage", "Home");
        }

        return View(new ReplenishmentDetailPageViewModel
        {
            Recommendation = recommendation,
            ReviewForm = new ReviewRecommendationFormViewModel { RecommendationId = id },
            StatusMessage = TempData["StatusMessage"] as string
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("replenishment:write")]
    public async Task<IActionResult> Review(ReviewRecommendationFormViewModel model, CancellationToken cancellationToken)
    {
        var recommendation = await GetRecommendationAsync(model.RecommendationId, cancellationToken);
        if (recommendation is null)
        {
            return RedirectToAction("NotFoundPage", "Home");
        }

        try
        {
            await _replenishmentApiClient.ReviewRecommendationAsync(model.RecommendationId, model.Action, model.Notes, cancellationToken);
            TempData["StatusMessage"] = "Recommendation updated successfully.";
            return RedirectToAction(nameof(Detail), new { id = model.RecommendationId });
        }
        catch (Exception exception)
        {
            return View("Detail", new ReplenishmentDetailPageViewModel
            {
                Recommendation = recommendation,
                ReviewForm = model,
                ErrorMessage = exception.Message
            });
        }
    }

    private async Task<ReplenishmentPageViewModel> BuildPageModelAsync(CancellationToken cancellationToken)
    {
        return new ReplenishmentPageViewModel
        {
            Recommendations = await _replenishmentApiClient.GetRecommendationsAsync(cancellationToken),
            StatusMessage = TempData["StatusMessage"] as string
        };
    }

    private async Task<ReplenishmentRecommendationViewModel?> GetRecommendationAsync(Guid id, CancellationToken cancellationToken)
    {
        var recommendations = await _replenishmentApiClient.GetRecommendationsAsync(cancellationToken);
        return recommendations.FirstOrDefault(item => item.Id == id);
    }
}
