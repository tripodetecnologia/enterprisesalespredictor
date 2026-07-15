using System.ComponentModel.DataAnnotations;

namespace EnterpriseSalesPredictor.Web.ViewModels.Replenishment;

public sealed class GenerateRecommendationFormViewModel
{
    [Required]
    public Guid? ProductId { get; set; }
}
