using EnterpriseSalesPredictor.Web.Filters;
using EnterpriseSalesPredictor.Web.Services;
using EnterpriseSalesPredictor.Web.ViewModels.Audit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Web.Controllers;

[Authorize]
[RequirePermission(Permissions.AuditRead)]
public sealed class AuditController : Controller
{
    private readonly AuditApiClient _auditApiClient;
    private const int SectionPageSize = 10;

    public AuditController(AuditApiClient auditApiClient)
    {
        _auditApiClient = auditApiClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] AuditFilterViewModel filters, int uploadPage = 1, int exportPage = 1, int functionalPage = 1, CancellationToken cancellationToken = default)
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
            UploadLogs = BuildSection(uploadLogs, uploadPage),
            ExportLogs = BuildSection(exportLogs, exportPage),
            FunctionalLogs = BuildSection(functionalLogs, functionalPage)
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

    private static PagedAuditSectionViewModel BuildSection(IReadOnlyCollection<AuditLogItemViewModel> items, int pageNumber)
    {
        var safePage = Math.Max(pageNumber, 1);
        var totalCount = items.Count;
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)SectionPageSize);

        return new PagedAuditSectionViewModel
        {
            Items = items.Skip((safePage - 1) * SectionPageSize).Take(SectionPageSize).ToArray(),
            PageNumber = safePage,
            PageSize = SectionPageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}
