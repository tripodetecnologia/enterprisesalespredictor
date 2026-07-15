namespace EnterpriseSalesPredictor.Application.Interfaces.Uploads;

public sealed class UploadRecordData
{
    public string InvoiceNumber { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string CustomerIdentification { get; set; } = string.Empty;

    public string CustomerAddress { get; set; } = string.Empty;

    public string CustomerCity { get; set; } = string.Empty;

    public string CustomerPhone { get; set; } = string.Empty;

    public string CustomerZone { get; set; } = string.Empty;

    public string ProductType { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string ProductReference { get; set; } = string.Empty;

    public string ProductBrand { get; set; } = string.Empty;

    public decimal ProductPurchasePrice { get; set; }

    public decimal ProductSalePrice { get; set; }

    public int ProductAvailableUnits { get; set; }

    public decimal QuantitySold { get; set; }

    public decimal SaleAmount { get; set; }

    public DateTime SaleDate { get; set; }

    public string SellerName { get; set; } = string.Empty;

    public string SellerIdentification { get; set; } = string.Empty;

    public string SupplierName { get; set; } = string.Empty;

    public string SupplierIdentification { get; set; } = string.Empty;

    public string SupplierAddress { get; set; } = string.Empty;

    public string SupplierPhone { get; set; } = string.Empty;

    public string SupplierCity { get; set; } = string.Empty;

    public string InvoicePaymentMethod { get; set; } = string.Empty;
}
