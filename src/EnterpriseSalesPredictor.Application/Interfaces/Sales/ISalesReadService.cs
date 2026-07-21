using EnterpriseSalesPredictor.Application.DTOs.Sales;

namespace EnterpriseSalesPredictor.Application.Interfaces.Sales;

public interface ISalesReadService
{
    Task<PagedSalesResult> QuerySalesAsync(SalesQueryCriteria criteria, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SalesDimensionSummaryDto>> GetSalesByCustomerAsync(SalesQueryCriteria criteria, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SalesDimensionSummaryDto>> GetSalesByProductAsync(SalesQueryCriteria criteria, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SalesDimensionSummaryDto>> GetSalesBySupplierAsync(SalesQueryCriteria criteria, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SalesDimensionSummaryDto>> GetSalesBySellerAsync(SalesQueryCriteria criteria, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SalesDimensionSummaryDto>> GetSalesByLocationAsync(SalesQueryCriteria criteria, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SalesComparisonSummaryDto>> GetSalesComparisonsAsync(string period, SalesQueryCriteria criteria, CancellationToken cancellationToken = default);
}
