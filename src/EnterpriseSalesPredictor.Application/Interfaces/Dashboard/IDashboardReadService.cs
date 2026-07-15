using EnterpriseSalesPredictor.Application.DTOs.Dashboard;

namespace EnterpriseSalesPredictor.Application.Interfaces.Dashboard;

public interface IDashboardReadService
{
    Task<DashboardKpiDto> GetKpisAsync(DashboardQueryCriteria criteria, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DashboardBreakdownItemDto>> GetTopCustomersAsync(DashboardQueryCriteria criteria, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DashboardBreakdownItemDto>> GetTopProductsAsync(DashboardQueryCriteria criteria, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DashboardBreakdownItemDto>> GetSalesByProductLineAsync(DashboardQueryCriteria criteria, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DashboardBreakdownItemDto>> GetSalesBySupplierAsync(DashboardQueryCriteria criteria, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DashboardAlertDto>> GetCommercialAlertsAsync(DashboardQueryCriteria criteria, CancellationToken cancellationToken = default);
}
