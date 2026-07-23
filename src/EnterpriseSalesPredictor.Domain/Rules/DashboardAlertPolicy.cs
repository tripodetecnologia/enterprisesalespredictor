namespace EnterpriseSalesPredictor.Domain.Rules;

public static class DashboardAlertPolicy
{
    public const int DefaultBreakdownLimit = 5;
    public const int MaximumBreakdownLimit = 20;
    public const int LowStockAlertLimit = 3;
    public const int PreviousSalesWindowDays = 14;
    public const int RecentSalesWindowDays = 7;

    public const decimal LowStockUnits = 5m;
    public const decimal CustomerConcentrationThreshold = 0.5m;
    public const decimal WeeklySlowdownMultiplier = 0.8m;
    public const decimal WeeklySlowdownPercent = 20m;
}
