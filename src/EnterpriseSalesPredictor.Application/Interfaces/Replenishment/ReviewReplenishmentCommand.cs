namespace EnterpriseSalesPredictor.Application.Interfaces.Replenishment;

public sealed class ReviewReplenishmentCommand
{
    public Guid RecommendationId { get; set; }

    public string Reviewer { get; set; } = string.Empty;

    public string ReviewerRole { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string? Notes { get; set; }
}
