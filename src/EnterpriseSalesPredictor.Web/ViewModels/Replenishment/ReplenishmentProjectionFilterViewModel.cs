using System.ComponentModel.DataAnnotations;

namespace EnterpriseSalesPredictor.Web.ViewModels.Replenishment;

public sealed class ReplenishmentProjectionFilterViewModel
{
    [Required]
    public DateTime? FromDate { get; set; }

    [Required]
    public DateTime? ToDate { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? ProductId { get; set; }
}
