namespace EnterpriseSalesPredictor.Web.ViewModels.Dashboard;

public sealed class DashboardBreakdownItemViewModel
{
    public string Label { get; set; } = string.Empty;

    public decimal TotalSalesAmount { get; set; }

    public decimal TotalQuantity { get; set; }

    public int TotalTransactions { get; set; }
}
