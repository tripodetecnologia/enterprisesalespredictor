namespace EnterpriseSalesPredictor.Domain.Rules;

public static class SaleRules
{
    public static bool IsValidAmount(decimal quantity, decimal amount)
    {
        return quantity > 0 && amount > 0;
    }

    public static bool HasValidDate(DateTime saleDate)
    {
        return saleDate <= DateTime.UtcNow.AddMinutes(5);
    }
}
