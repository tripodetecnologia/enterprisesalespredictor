using System.ComponentModel.DataAnnotations;

namespace EnterpriseSalesPredictor.Web.ViewModels.Replenishment;

public sealed class ReviewRecommendationFormViewModel
{
    [Required]
    public Guid RecommendationId { get; set; }

    [Required]
    public string Action { get; set; } = string.Empty;

    public string? Notes { get; set; }
}
