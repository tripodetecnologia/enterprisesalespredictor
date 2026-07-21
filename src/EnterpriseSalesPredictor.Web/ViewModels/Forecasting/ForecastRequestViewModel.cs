using System.ComponentModel.DataAnnotations;

namespace EnterpriseSalesPredictor.Web.ViewModels.Forecasting;

public sealed class ForecastRequestViewModel
{
    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    public DateTime? FromDate { get; set; }

    [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
    public DateTime? ToDate { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? CustomerId { get; set; }
}
