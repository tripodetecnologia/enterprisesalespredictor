namespace EnterpriseSalesPredictor.Application.DTOs.Dashboard;

public sealed class DashboardKpiDto
{
    public decimal TotalSalesAmount { get; set; }

    public decimal TotalQuantity { get; set; }

    public int TotalTransactions { get; set; }

    public decimal AverageTicket { get; set; }
}
