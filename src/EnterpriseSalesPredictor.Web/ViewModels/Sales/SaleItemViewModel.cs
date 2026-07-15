namespace EnterpriseSalesPredictor.Web.ViewModels.Sales;

public sealed class SaleItemViewModel
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
