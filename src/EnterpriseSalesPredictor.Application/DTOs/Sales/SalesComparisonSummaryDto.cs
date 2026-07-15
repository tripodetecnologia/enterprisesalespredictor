namespace EnterpriseSalesPredictor.Application.DTOs.Sales;

public sealed class SalesComparisonSummaryDto
{
    public string PeriodType { get; set; } = string.Empty;

    public string PeriodKey { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public decimal TotalQuantity { get; set; }

    public int Transactions { get; set; }
}
