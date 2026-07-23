using EnterpriseSalesPredictor.Web.Filters;
using EnterpriseSalesPredictor.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Web.Controllers;

[Authorize]
[RequirePermission(Permissions.ExportsWrite)]
public sealed class ExportsController : Controller
{
    private readonly ExportsApiClient _exportsApiClient;

    public ExportsController(ExportsApiClient exportsApiClient)
    {
        _exportsApiClient = exportsApiClient;
    }

    [HttpGet]
    public async Task<IActionResult> Reports(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken)
    {
        var file = await _exportsApiClient.ExportReportsAsync(fromDate, toDate, cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpGet]
    public async Task<IActionResult> FilteredSales(
        DateTime? fromDate,
        DateTime? toDate,
        string? city,
        string? zone,
        int pageNumber,
        int pageSize,
        string? sortBy,
        string? sortDirection,
        CancellationToken cancellationToken)
    {
        var query = new Dictionary<string, string?>
        {
            ["FromDate"] = fromDate?.ToString("o"),
            ["ToDate"] = toDate?.ToString("o"),
            ["City"] = city,
            ["Zone"] = zone,
            ["PageNumber"] = pageNumber.ToString(),
            ["PageSize"] = pageSize.ToString(),
            ["SortBy"] = sortBy,
            ["SortDirection"] = sortDirection
        };

        var file = await _exportsApiClient.ExportFilteredSalesAsync(query, cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpGet]
    public async Task<IActionResult> BaseData(CancellationToken cancellationToken)
    {
        var file = await _exportsApiClient.ExportBaseDataAsync(cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }
}
