namespace EnterpriseSalesPredictor.Domain.Entities;

public sealed class Product : Entity
{
    public Product(
        Guid id,
        string productType,
        string name,
        string reference,
        string brand,
        decimal purchasePrice,
        decimal salePrice,
        int availableUnits)
        : base(id)
    {
        ProductType = productType;
        Name = name;
        Reference = reference;
        Brand = brand;
        PurchasePrice = purchasePrice;
        SalePrice = salePrice;
        AvailableUnits = availableUnits;
    }

    public string ProductType { get; private set; }

    public string Name { get; private set; }

    public string Reference { get; private set; }

    public string Brand { get; private set; }

    public decimal PurchasePrice { get; private set; }

    public decimal SalePrice { get; private set; }

    public int AvailableUnits { get; private set; }

    public void UpdateAvailableUnits(int availableUnits)
    {
        AvailableUnits = availableUnits;
    }
}
