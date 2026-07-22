using System.ComponentModel.DataAnnotations;

namespace EnterpriseSalesPredictor.Web.ViewModels.Replenishment;

public sealed class ReplenishmentProjectionFilterViewModel
{
    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    public DateTime? FromDate { get; set; }

    [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
    public DateTime? ToDate { get; set; }

    public Guid? CustomerId { get; set; }

    [Required(ErrorMessage = "El producto es obligatorio.")]
    public string? ProductName { get; set; }
}
