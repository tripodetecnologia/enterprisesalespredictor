namespace EnterpriseSalesPredictor.Application.Interfaces.Replenishment;

public sealed class SubmitReplenishmentProjectionCommand
{
    public DateTime ProjectionMonth { get; set; }

    public Guid ProductId { get; set; }

    public decimal RecommendedUnits { get; set; }

    public int CurrentStockUnits { get; set; }

    public string RequestedBy { get; set; } = string.Empty;
}
