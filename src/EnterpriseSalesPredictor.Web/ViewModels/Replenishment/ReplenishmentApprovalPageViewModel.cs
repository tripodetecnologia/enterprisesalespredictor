namespace EnterpriseSalesPredictor.Web.ViewModels.Replenishment;

public sealed class ReplenishmentApprovalPageViewModel
{
    public PagedReplenishmentResultViewModel Recommendations { get; set; } = new();

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public Guid? ProductId { get; set; }

    public IReadOnlyCollection<Forecasting.ForecastOptionViewModel> Products { get; set; } = Array.Empty<Forecasting.ForecastOptionViewModel>();

    public string? StatusMessage { get; set; }

    public string? ErrorMessage { get; set; }
}
