namespace EnterpriseSalesPredictor.Application.DTOs.Forecasting;

public sealed class ForecastDto
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
