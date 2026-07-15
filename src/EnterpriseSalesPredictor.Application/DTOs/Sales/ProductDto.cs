namespace EnterpriseSalesPredictor.Application.DTOs.Sales;

public sealed class ProductDto
{
    public Guid Id { get; set; }

    public string ProductType { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Reference { get; set; } = string.Empty;

    public string Brand { get; set; } = string.Empty;

    public decimal PurchasePrice { get; set; }

    public decimal SalePrice { get; set; }

    public int AvailableUnits { get; set; }
}
