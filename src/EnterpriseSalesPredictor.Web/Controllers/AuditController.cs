using EnterpriseSalesPredictor.Web.Filters;
using EnterpriseSalesPredictor.Web.Services;
using EnterpriseSalesPredictor.Web.ViewModels.Audit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Web.Controllers;

[Authorize]
[RequirePermission("audit:read")]
public sealed class AuditController : Controller
{
    private readonly AuditApiClient _auditApiClient;

    public AuditController(AuditApiClient auditApiClient)
    {
        _auditApiClient = auditApiClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] AuditFilterViewModel filters, CancellationToken cancellationToken)
    {
        var allLogs = await _auditApiClient.GetAuditLogsAsync(cancellationToken);
        var filtered = ApplyFilters(allLogs, filters);

        var uploadLogs = filtered
            .Where(log => string.Equals(log.Module, "Uploads", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var exportLogs = filtered
            .Where(log => string.Equals(log.Module, "Exports", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var functionalLogs = filtered
            .Where(log => !string.Equals(log.Module, "Uploads", StringComparison.OrdinalIgnoreCase)
                          && !string.Equals(log.Module, "Exports", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var viewModel = new AuditPageViewModel
        {
            Filters = filters,
            UploadLogs = uploadLogs,
            ExportLogs = exportLogs,
            FunctionalLogs = functionalLogs
        };

        return View(viewModel);
    }

    private static IReadOnlyCollection<AuditLogItemViewModel> ApplyFilters(
        IReadOnlyCollection<AuditLogItemViewModel> logs,
        AuditFilterViewModel filters)
    {
        var query = logs.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(filters.Module))
        {
            query = query.Where(log => log.Module.Contains(filters.Module, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(filters.Actor))
        {
            query = query.Where(log => log.Actor.Contains(filters.Actor, StringComparison.OrdinalIgnoreCase));
        }

        if (filters.FromUtc.HasValue)
        {
            query = query.Where(log => log.OccurredAtUtc >= filters.FromUtc.Value);
        }

        if (filters.ToUtc.HasValue)
        {
            query = query.Where(log => log.OccurredAtUtc <= filters.ToUtc.Value);
        }

        return query
            .OrderByDescending(log => log.OccurredAtUtc)
            .ToArray();
    }
}
