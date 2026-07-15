namespace EnterpriseSalesPredictor.Domain.Rules;

public static class ReplenishmentRules
{
    private static readonly HashSet<string> AllowedApproverRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "PurchaseManager",
        "WarehouseManager"
    };

    public static bool CanApprove(string role)
    {
        return AllowedApproverRoles.Contains(role);
    }

    public static bool ShouldGenerateRecommendation(decimal projectedDemand, int availableUnits)
    {
        return projectedDemand > availableUnits;
    }
}
