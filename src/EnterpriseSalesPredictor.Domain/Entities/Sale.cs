namespace EnterpriseSalesPredictor.Domain.Entities;

public sealed class Sale : Entity
{
    public Sale(
        Guid id,
        string invoiceNumber,
        Guid customerId,
        Guid productId,
        Guid supplierId,
        Guid sellerId,
        DateTime saleDate,
        decimal quantity,
        decimal saleAmount,
        string paymentMethod)
        : base(id)
    {
        InvoiceNumber = invoiceNumber;
        CustomerId = customerId;
        ProductId = productId;
        SupplierId = supplierId;
        SellerId = sellerId;
        SaleDate = saleDate;
        Quantity = quantity;
        SaleAmount = saleAmount;
        PaymentMethod = paymentMethod;
    }

    public string InvoiceNumber { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid ProductId { get; private set; }

    public Guid SupplierId { get; private set; }

    public Guid SellerId { get; private set; }

    public DateTime SaleDate { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal SaleAmount { get; private set; }

    public string PaymentMethod { get; private set; }
}
