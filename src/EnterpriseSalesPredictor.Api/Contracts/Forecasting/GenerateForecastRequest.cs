using System.ComponentModel.DataAnnotations;

namespace EnterpriseSalesPredictor.Api.Contracts.Forecasting;

public sealed class GenerateForecastRequest
{
    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    public DateTime? FromDate { get; set; }

    [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
    public DateTime? ToDate { get; set; }

    public string? ProductName { get; set; }

    public Guid? CustomerId { get; set; }
}
