namespace EnterpriseSalesPredictor.Domain.Entities;

public sealed class Forecast : Entity
{
    public Forecast(
        Guid id,
        DateTime generatedAtUtc,
        DateTime fromDate,
        DateTime toDate,
        decimal projectedSales,
        decimal confidence,
        string generatedBy)
        : base(id)
    {
        GeneratedAtUtc = generatedAtUtc;
        FromDate = fromDate;
        ToDate = toDate;
        ProjectedSales = projectedSales;
        Confidence = confidence;
        GeneratedBy = generatedBy;
    }

    public DateTime GeneratedAtUtc { get; private set; }

    public DateTime FromDate { get; private set; }

    public DateTime ToDate { get; private set; }

    public decimal ProjectedSales { get; private set; }

    public decimal Confidence { get; private set; }

    public string GeneratedBy { get; private set; }
}
