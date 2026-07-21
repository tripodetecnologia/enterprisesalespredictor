namespace EnterpriseSalesPredictor.Web.ViewModels.Replenishment;

public sealed class ReplenishmentPageViewModel
{
    public PagedReplenishmentResultViewModel Recommendations { get; set; } = new();

    public GenerateRecommendationFormViewModel GenerateForm { get; set; } = new();

    public string? StatusMessage { get; set; }

    public string? ErrorMessage { get; set; }
}
