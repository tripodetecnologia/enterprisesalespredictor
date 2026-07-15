namespace EnterpriseSalesPredictor.Api.Contracts.Audit;

public sealed class RegisterRecommendationAuditRequest
{
    public Guid ProductId { get; set; }

    public decimal RecommendedUnits { get; set; }
}
