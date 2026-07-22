namespace EnterpriseSalesPredictor.Web.ViewModels.Replenishment;

public sealed class ReplenishmentRecommendationViewModel
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string ProductType { get; set; } = string.Empty;

    public string ProductReference { get; set; } = string.Empty;

    public string ProductBrand { get; set; } = string.Empty;

    public DateTime GeneratedAtUtc { get; set; }

    public DateTime RecommendedForMonth { get; set; }

    public decimal RecommendedUnits { get; set; }

    public decimal Confidence { get; set; }

    public string Rationale { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime? ReviewedAtUtc { get; set; }

    public string? ReviewedBy { get; set; }

    public string? ReviewNotes { get; set; }
}
