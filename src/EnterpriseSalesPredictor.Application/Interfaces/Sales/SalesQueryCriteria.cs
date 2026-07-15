namespace EnterpriseSalesPredictor.Application.Interfaces.Sales;

public sealed class SalesQueryCriteria
{
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? SupplierId { get; set; }

    public Guid? SellerId { get; set; }

    public string? City { get; set; }

    public string? Zone { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 50;

    public string SortBy { get; set; } = "SaleDate";

    public string SortDirection { get; set; } = "desc";
}
