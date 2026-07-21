namespace EnterpriseSalesPredictor.Api.Contracts.Replenishment;

public sealed class SubmitProjectionRequest
{
    public DateTime ProjectionMonth { get; set; }

    public Guid ProductId { get; set; }

    public decimal RecommendedUnits { get; set; }

    public int CurrentStockUnits { get; set; }
}
