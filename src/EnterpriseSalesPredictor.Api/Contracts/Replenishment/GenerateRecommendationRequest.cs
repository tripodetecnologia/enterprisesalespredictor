namespace EnterpriseSalesPredictor.Api.Contracts.Replenishment;

public sealed class GenerateRecommendationRequest
{
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }
}
