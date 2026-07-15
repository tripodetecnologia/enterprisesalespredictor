namespace EnterpriseSalesPredictor.Application.Interfaces.Replenishment;

public sealed class GenerateReplenishmentCommand
{
    public Guid ProductId { get; set; }

    public decimal ProjectedDemand { get; set; }

    public int AvailableUnits { get; set; }

    public string RequestedBy { get; set; } = string.Empty;
}
