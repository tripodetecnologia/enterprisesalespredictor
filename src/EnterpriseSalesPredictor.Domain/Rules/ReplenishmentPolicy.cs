namespace EnterpriseSalesPredictor.Domain.Rules;

public static class ReplenishmentPolicy
{
    public const int ProjectionLookbackDays = 365;
    public const int RecommendationLookbackDays = 90;
    public const int DefaultPageSize = 10;

    public const decimal MinimumStockUnits = 5m;
    public const decimal LowRotationUnits = 2m;
    public const decimal StockoutRiskDemandRatio = 0.25m;
    public const decimal RecommendedBufferRatio = 0.15m;

    public const decimal LowRotationConfidence = 0.55m;
    public const decimal StandardConfidence = 0.72m;
    public const decimal StockoutRiskConfidence = 0.82m;
}
