namespace EnterpriseSalesPredictor.Web.ViewModels.Replenishment;

public sealed class ReplenishmentProjectionViewModel
{
    public DateTime ProjectionMonth { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string ProductType { get; set; } = string.Empty;

    public string ProductReference { get; set; } = string.Empty;

    public string ProductBrand { get; set; } = string.Empty;

    public decimal RecommendedUnits { get; set; }

    public int CurrentStockUnits { get; set; }
}
