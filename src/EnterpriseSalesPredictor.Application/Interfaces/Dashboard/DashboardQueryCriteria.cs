namespace EnterpriseSalesPredictor.Application.Interfaces.Dashboard;

public sealed class DashboardQueryCriteria
{
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public int Limit { get; set; } = 5;
}
