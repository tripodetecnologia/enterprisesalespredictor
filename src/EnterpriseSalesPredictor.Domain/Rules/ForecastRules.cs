namespace EnterpriseSalesPredictor.Domain.Rules;

public static class ForecastRules
{
    public const int MinimumForecastDays = 1;
    public const int MaximumForecastDays = 365;

    public static bool IsValidForecastRange(DateTime fromDate, DateTime toDate)
    {
        if (toDate < fromDate)
        {
            return false;
        }

        var days = (toDate.Date - fromDate.Date).TotalDays + 1;
        return days >= MinimumForecastDays && days <= MaximumForecastDays;
    }
}
