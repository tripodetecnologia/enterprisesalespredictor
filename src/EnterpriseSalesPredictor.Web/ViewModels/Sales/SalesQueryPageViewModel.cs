namespace EnterpriseSalesPredictor.Web.ViewModels.Sales;

public sealed class SalesQueryPageViewModel
{
    public SalesQueryFilterViewModel Filters { get; set; } = new();

    public IReadOnlyCollection<SaleItemViewModel> Results { get; set; } = Array.Empty<SaleItemViewModel>();
}
