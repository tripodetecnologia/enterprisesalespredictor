using EnterpriseSalesPredictor.Application.Interfaces.Auditing;
using EnterpriseSalesPredictor.Application.Interfaces.Exports;
using EnterpriseSalesPredictor.Application.Interfaces.Reports;
using EnterpriseSalesPredictor.Application.Interfaces.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Api.Controllers;

[ApiController]
[Route("api/exports")]
[Authorize]
[Authorize(Policy = PermissionPolicies.ExportsWrite)]
public sealed class ExportsController : ControllerBase
{
    private readonly IExportService _exportService;
    private readonly IAuditLogService _auditLogService;

    public ExportsController(IExportService exportService, IAuditLogService auditLogService)
    {
        _exportService = exportService;
        _auditLogService = auditLogService;
    }

    [HttpGet("reports")]
    public async Task<IActionResult> ExportReportsAsync([FromQuery] ReportQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var file = await _exportService.ExportReportsAsync(criteria, cancellationToken);
        await RegisterAuditAsync("ReportsExported", $"Type=reports; File={file.FileName}", cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpGet("filtered-sales")]
    public async Task<IActionResult> ExportFilteredSalesAsync([FromQuery] SalesQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var file = await _exportService.ExportFilteredSalesAsync(criteria, cancellationToken);
        await RegisterAuditAsync("FilteredDataExported", $"Type=filtered-sales; File={file.FileName}", cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpGet("base-data")]
    public async Task<IActionResult> ExportBaseDataAsync(CancellationToken cancellationToken)
    {
        var file = await _exportService.ExportBaseDataAsync(cancellationToken);
        await RegisterAuditAsync("BaseDataExported", $"Type=base-data; File={file.FileName}", cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }

    private async Task RegisterAuditAsync(string action, string details, CancellationToken cancellationToken)
    {
        await _auditLogService.RecordAsync(new CreateAuditLogCommand
        {
            Actor = User.Identity?.Name ?? "system",
            Action = action,
            Module = "Exports",
            Details = details
        }, cancellationToken);
    }
}
