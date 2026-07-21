namespace EnterpriseSalesPredictor.Application.Interfaces.Replenishment;

public sealed class GenerateReplenishmentCommand
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public string RequestedBy { get; set; } = string.Empty;
}
