using EnterpriseSalesPredictor.Api.Contracts.Replenishment;
using EnterpriseSalesPredictor.Application.Interfaces.Replenishment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Api.Controllers;

[ApiController]
[Route("api/replenishment")]
[Authorize]
public sealed class ReplenishmentController : ControllerBase
{
    private readonly IReplenishmentService _replenishmentService;

    public ReplenishmentController(IReplenishmentService replenishmentService)
    {
        _replenishmentService = replenishmentService;
    }

    [HttpGet("recommendations")]
    [Authorize(Policy = "Permission:replenishment:read")]
    public async Task<IActionResult> GetRecommendationsAsync([FromQuery] ReplenishmentQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _replenishmentService.GetRecommendationsAsync(criteria, cancellationToken);
        return Ok(result);
    }

    [HttpGet("projections")]
    [Authorize(Policy = "Permission:replenishment:read")]
    public async Task<IActionResult> GetProjectionsAsync([FromQuery] ReplenishmentProjectionQueryCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _replenishmentService.GetProjectionsAsync(criteria, cancellationToken);
        return Ok(result);
    }

    [HttpPost("recommendations")]
    [Authorize(Policy = "Permission:replenishment:write")]
    public async Task<IActionResult> GenerateRecommendationAsync([FromBody] GenerateRecommendationRequest request, CancellationToken cancellationToken)
    {
        if (!request.FromDate.HasValue || !request.ToDate.HasValue)
        {
            return BadRequest(new { message = "Debés indicar un rango de fechas válido." });
        }

        var result = await _replenishmentService.GenerateRecommendationAsync(new GenerateReplenishmentCommand
        {
            FromDate = request.FromDate.Value,
            ToDate = request.ToDate.Value,
            RequestedBy = User.Identity?.Name ?? "system"
        }, cancellationToken);

        return Ok(result);
    }

    [HttpPost("projections/submit")]
    [Authorize(Policy = "Permission:replenishment:write")]
    public async Task<IActionResult> SubmitProjectionAsync([FromBody] SubmitProjectionRequest request, CancellationToken cancellationToken)
    {
        var result = await _replenishmentService.SubmitProjectionAsync(new SubmitReplenishmentProjectionCommand
        {
            ProjectionMonth = request.ProjectionMonth,
            ProductId = request.ProductId,
            RecommendedUnits = request.RecommendedUnits,
            CurrentStockUnits = request.CurrentStockUnits,
            RequestedBy = User.Identity?.Name ?? "system"
        }, cancellationToken);

        return Ok(result);
    }

    [HttpPost("recommendations/{id:guid}/approve")]
    [Authorize(Policy = "Permission:replenishment:write")]
    public async Task<IActionResult> ApproveRecommendationAsync(Guid id, [FromBody] ReviewRecommendationRequest request, CancellationToken cancellationToken)
    {
        var result = await ReviewAsync(id, "approve", request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("recommendations/{id:guid}/reject")]
    [Authorize(Policy = "Permission:replenishment:write")]
    public async Task<IActionResult> RejectRecommendationAsync(Guid id, [FromBody] ReviewRecommendationRequest request, CancellationToken cancellationToken)
    {
        var result = await ReviewAsync(id, "reject", request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("recommendations/{id:guid}/analysis")]
    [Authorize(Policy = "Permission:replenishment:write")]
    public async Task<IActionResult> MarkRecommendationForAnalysisAsync(Guid id, [FromBody] ReviewRecommendationRequest request, CancellationToken cancellationToken)
    {
        var result = await ReviewAsync(id, "analysis", request, cancellationToken);
        return Ok(result);
    }

    private Task<EnterpriseSalesPredictor.Application.DTOs.Replenishment.ReplenishmentRecommendationDto> ReviewAsync(
        Guid id,
        string action,
        ReviewRecommendationRequest request,
        CancellationToken cancellationToken)
    {
        return _replenishmentService.ReviewRecommendationAsync(new ReviewReplenishmentCommand
        {
            RecommendationId = id,
            Reviewer = User.Identity?.Name ?? "system",
            ReviewerRole = User.Claims.FirstOrDefault(claim => claim.Type == System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty,
            Action = action,
            Notes = request.Notes
        }, cancellationToken);
    }
}
