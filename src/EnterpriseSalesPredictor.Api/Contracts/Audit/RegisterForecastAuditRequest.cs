namespace EnterpriseSalesPredictor.Api.Contracts.Audit;

public sealed class RegisterForecastAuditRequest
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }
}
