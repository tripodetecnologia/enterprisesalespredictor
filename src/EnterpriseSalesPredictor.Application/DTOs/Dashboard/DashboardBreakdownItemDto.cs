namespace EnterpriseSalesPredictor.Application.DTOs.Dashboard;

public sealed class DashboardBreakdownItemDto
{
    public string Label { get; set; } = string.Empty;

    public decimal TotalSalesAmount { get; set; }

    public decimal TotalQuantity { get; set; }

    public int TotalTransactions { get; set; }
}
