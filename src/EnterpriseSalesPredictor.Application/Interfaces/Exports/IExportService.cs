using EnterpriseSalesPredictor.Application.Interfaces.Reports;
using EnterpriseSalesPredictor.Application.Interfaces.Sales;

namespace EnterpriseSalesPredictor.Application.Interfaces.Exports;

public interface IExportService
{
    Task<ExportFileDto> ExportReportsAsync(ReportQueryCriteria criteria, CancellationToken cancellationToken = default);

    Task<ExportFileDto> ExportFilteredSalesAsync(SalesQueryCriteria criteria, CancellationToken cancellationToken = default);

    Task<ExportFileDto> ExportBaseDataAsync(CancellationToken cancellationToken = default);
}
