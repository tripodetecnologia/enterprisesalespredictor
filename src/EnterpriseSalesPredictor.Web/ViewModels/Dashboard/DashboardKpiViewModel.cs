namespace EnterpriseSalesPredictor.Web.ViewModels.Dashboard;

public sealed class DashboardKpiViewModel
{
    public decimal TotalSalesAmount { get; set; }

    public decimal TotalQuantity { get; set; }

    public int TotalTransactions { get; set; }

    public decimal AverageTicket { get; set; }
}
