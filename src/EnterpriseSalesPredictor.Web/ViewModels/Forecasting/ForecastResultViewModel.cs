namespace EnterpriseSalesPredictor.Web.ViewModels.Forecasting;

public sealed class ForecastResultViewModel
{
    public Guid Id { get; set; }

    public DateTime GeneratedAtUtc { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public decimal ProjectedSales { get; set; }

    public decimal Confidence { get; set; }

    public string GeneratedBy { get; set; } = string.Empty;

    public string Explanation { get; set; } = string.Empty;
}
