namespace EnterpriseSalesPredictor.Application.DTOs.Sales;

public sealed class SalesDimensionSummaryDto
{
    public string DimensionKey { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public decimal TotalQuantity { get; set; }

    public int Transactions { get; set; }
}
