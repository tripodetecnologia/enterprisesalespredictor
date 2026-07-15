namespace EnterpriseSalesPredictor.Web.ViewModels.Replenishment;

public sealed class ReplenishmentPageViewModel
{
    public IReadOnlyCollection<ReplenishmentRecommendationViewModel> Recommendations { get; set; } = Array.Empty<ReplenishmentRecommendationViewModel>();

    public GenerateRecommendationFormViewModel GenerateForm { get; set; } = new();

    public string? StatusMessage { get; set; }

    public string? ErrorMessage { get; set; }
}
