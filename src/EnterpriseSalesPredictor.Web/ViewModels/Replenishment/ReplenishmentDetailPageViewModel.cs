namespace EnterpriseSalesPredictor.Web.ViewModels.Replenishment;

public sealed class ReplenishmentDetailPageViewModel
{
    public ReplenishmentRecommendationViewModel Recommendation { get; set; } = new();

    public ReviewRecommendationFormViewModel ReviewForm { get; set; } = new();

    public string? StatusMessage { get; set; }

    public string? ErrorMessage { get; set; }
}
