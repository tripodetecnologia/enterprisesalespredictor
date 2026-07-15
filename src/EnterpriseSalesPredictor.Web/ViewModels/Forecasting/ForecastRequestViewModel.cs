namespace EnterpriseSalesPredictor.Web.ViewModels.Forecasting;

public sealed class ForecastRequestViewModel
{
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? CustomerId { get; set; }
}
