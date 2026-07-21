using EnterpriseSalesPredictor.Application.DTOs.Sales;

namespace EnterpriseSalesPredictor.Application.Interfaces.Sales;

public sealed class PagedSalesResult
{
    public IReadOnlyCollection<SaleDto> Items { get; set; } = Array.Empty<SaleDto>();

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}
