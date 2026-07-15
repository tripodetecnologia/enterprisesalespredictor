namespace EnterpriseSalesPredictor.Application.DTOs.Sales;

public sealed class SaleDto
{
    public Guid Id { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }

    public Guid ProductId { get; set; }

    public Guid SupplierId { get; set; }

    public Guid SellerId { get; set; }

    public DateTime SaleDate { get; set; }

    public decimal Quantity { get; set; }

    public decimal SaleAmount { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;
}
