namespace EnterpriseSalesPredictor.Domain.Rules;

public static class ForecastPolicy
{
    public const int LookbackHorizonMultiplier = 3;
    public const int MinimumLookbackDays = 90;
    public const int TopForecastItems = 6;

    public const int HighConfidenceActiveDays = 60;
    public const int MediumHighConfidenceActiveDays = 30;
    public const int MediumConfidenceActiveDays = 14;

    public const decimal HighConfidence = 0.90m;
    public const decimal MediumHighConfidence = 0.80m;
    public const decimal MediumConfidence = 0.70m;
    public const decimal LowConfidence = 0.55m;
    public const decimal NoHistoryConfidence = 0.35m;
}
