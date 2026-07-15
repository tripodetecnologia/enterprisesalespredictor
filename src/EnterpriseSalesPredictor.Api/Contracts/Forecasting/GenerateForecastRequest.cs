namespace EnterpriseSalesPredictor.Api.Contracts.Forecasting;

public sealed class GenerateForecastRequest
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? CustomerId { get; set; }
}
