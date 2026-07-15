using EnterpriseSalesPredictor.Application.DTOs.Reports;

namespace EnterpriseSalesPredictor.Application.Interfaces.Reports;

public interface IReportReadService
{
    Task<ReportDto> GetManagementReportAsync(ReportQueryCriteria criteria, CancellationToken cancellationToken = default);

    Task<ReportDto> GetCommercialReportAsync(ReportQueryCriteria criteria, CancellationToken cancellationToken = default);

    Task<ReportDto> GetOperationalReportAsync(ReportQueryCriteria criteria, CancellationToken cancellationToken = default);

    Task<ReportDto> GetReplenishmentReportAsync(ReportQueryCriteria criteria, CancellationToken cancellationToken = default);

    Task<ReportDto> GetPredictiveReportAsync(ReportQueryCriteria criteria, CancellationToken cancellationToken = default);
}
