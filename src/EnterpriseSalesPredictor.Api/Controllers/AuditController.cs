using EnterpriseSalesPredictor.Api.Contracts.Audit;
using EnterpriseSalesPredictor.Application.Interfaces.Auditing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Api.Controllers;

[ApiController]
[Route("api/audit")]
[Authorize]
public sealed class AuditController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet]
    [Authorize(Policy = PermissionPolicies.AuditRead)]
    public async Task<IActionResult> GetAuditLogsAsync(CancellationToken cancellationToken)
    {
        var logs = await _auditLogService.GetAuditLogsAsync(cancellationToken);
        return Ok(logs);
    }

    [HttpPost("exports")]
    [Authorize(Policy = PermissionPolicies.ExportsWrite)]
    public async Task<IActionResult> RegisterExportAsync(RegisterExportAuditRequest request, CancellationToken cancellationToken)
    {
        var actor = GetActor();
        var details = $"ExportType={request.ExportType}; Filters={request.Filters}";

        var entry = await _auditLogService.RecordAsync(new CreateAuditLogCommand
        {
            Actor = actor,
            Action = "ExportGenerated",
            Module = "Exports",
            Details = details
        }, cancellationToken);

        return Ok(entry);
    }

    [HttpPost("forecasts")]
    [Authorize(Policy = PermissionPolicies.ForecastsWrite)]
    public async Task<IActionResult> RegisterForecastAsync(RegisterForecastAuditRequest request, CancellationToken cancellationToken)
    {
        var actor = GetActor();
        var details = $"FromDate={request.FromDate.ToString(DateFormats.HtmlDate)}; ToDate={request.ToDate.ToString(DateFormats.HtmlDate)}";

        var entry = await _auditLogService.RecordAsync(new CreateAuditLogCommand
        {
            Actor = actor,
            Action = "ForecastGenerated",
            Module = "Forecasting",
            Details = details
        }, cancellationToken);

        return Ok(entry);
    }

    [HttpPost("replenishment/recommendations")]
    [Authorize(Policy = PermissionPolicies.ReplenishmentWrite)]
    public async Task<IActionResult> RegisterRecommendationAsync(RegisterRecommendationAuditRequest request, CancellationToken cancellationToken)
    {
        var actor = GetActor();
        var details = $"ProductId={request.ProductId}; RecommendedUnits={request.RecommendedUnits}";

        var entry = await _auditLogService.RecordAsync(new CreateAuditLogCommand
        {
            Actor = actor,
            Action = "RecommendationGenerated",
            Module = "Replenishment",
            Details = details
        }, cancellationToken);

        return Ok(entry);
    }

    [HttpPost("replenishment/reviews")]
    [Authorize(Policy = PermissionPolicies.ReplenishmentWrite)]
    public async Task<IActionResult> RegisterRecommendationReviewAsync(ReviewRecommendationAuditRequest request, CancellationToken cancellationToken)
    {
        var actor = GetActor();
        var outcome = request.Approve ? "Approved" : "Rejected";
        var details = $"RecommendationId={request.RecommendationId}; Outcome={outcome}; Notes={request.Notes}";

        var entry = await _auditLogService.RecordAsync(new CreateAuditLogCommand
        {
            Actor = actor,
            Action = "RecommendationReviewed",
            Module = "Replenishment",
            Details = details
        }, cancellationToken);

        return Ok(entry);
    }

    private string GetActor()
    {
        return User.Identity?.Name ?? "system";
    }
}
