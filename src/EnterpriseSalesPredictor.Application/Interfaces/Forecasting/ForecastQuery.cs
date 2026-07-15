namespace EnterpriseSalesPredictor.Application.Interfaces.Forecasting;

public sealed class ForecastQuery
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? CustomerId { get; set; }

    public string RequestedBy { get; set; } = string.Empty;
}
