namespace EnterpriseSalesPredictor.Application.DTOs.Sales;

public sealed class SaleDto
{
    public Guid Id { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public Guid SupplierId { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public Guid SellerId { get; set; }

    public string SellerName { get; set; } = string.Empty;

    public DateTime SaleDate { get; set; }

    public decimal Quantity { get; set; }

    public decimal SaleAmount { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;
}
