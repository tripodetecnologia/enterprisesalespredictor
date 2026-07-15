namespace EnterpriseSalesPredictor.Api.Contracts.Audit;

public sealed class ReviewRecommendationAuditRequest
{
    public Guid RecommendationId { get; set; }

    public bool Approve { get; set; }

    public string Notes { get; set; } = string.Empty;
}
