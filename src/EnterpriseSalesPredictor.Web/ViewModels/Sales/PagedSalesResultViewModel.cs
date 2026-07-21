namespace EnterpriseSalesPredictor.Web.ViewModels.Sales;

public sealed class PagedSalesResultViewModel
{
    public IReadOnlyCollection<SaleItemViewModel> Items { get; set; } = Array.Empty<SaleItemViewModel>();

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}
