namespace EnterpriseSalesPredictor.Domain.Entities;

public sealed class ReplenishmentRecommendation : Entity
{
    public ReplenishmentRecommendation(
        Guid id,
        Guid productId,
        DateTime generatedAtUtc,
        DateTime recommendedForMonth,
        decimal recommendedUnits,
        decimal confidence,
        string rationale)
        : base(id)
    {
        ProductId = productId;
        GeneratedAtUtc = generatedAtUtc;
        RecommendedForMonth = recommendedForMonth;
        RecommendedUnits = recommendedUnits;
        Confidence = confidence;
        Rationale = rationale;
        Status = RecommendationStatus.Pending;
    }

    public Guid ProductId { get; private set; }

    public Product? Product { get; private set; }

    public DateTime GeneratedAtUtc { get; private set; }

    public DateTime RecommendedForMonth { get; private set; }

    public decimal RecommendedUnits { get; private set; }

    public decimal Confidence { get; private set; }

    public string Rationale { get; private set; }

    public RecommendationStatus Status { get; private set; }

    public DateTime? ReviewedAtUtc { get; private set; }

    public string? ReviewedBy { get; private set; }

    public string? ReviewNotes { get; private set; }

    public void Approve(string reviewedBy, string? notes)
    {
        Status = RecommendationStatus.Approved;
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewedBy = reviewedBy;
        ReviewNotes = notes;
    }

    public void Reject(string reviewedBy, string? notes)
    {
        Status = RecommendationStatus.Rejected;
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewedBy = reviewedBy;
        ReviewNotes = notes;
    }

    public void MarkForAnalysis(string reviewedBy, string? notes)
    {
        Status = RecommendationStatus.LowConfidence;
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewedBy = reviewedBy;
        ReviewNotes = notes;
    }

    public void Refresh(decimal recommendedUnits, decimal confidence, string rationale)
    {
        GeneratedAtUtc = DateTime.UtcNow;
        RecommendedUnits = recommendedUnits;
        Confidence = confidence;
        Rationale = rationale;
        Status = RecommendationStatus.Pending;
        ReviewedAtUtc = null;
        ReviewedBy = null;
        ReviewNotes = null;
    }
}
