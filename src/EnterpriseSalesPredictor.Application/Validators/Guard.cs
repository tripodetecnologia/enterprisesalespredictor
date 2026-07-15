namespace EnterpriseSalesPredictor.Application.Validators;

public static class Guard
{
    public static void AgainstNullOrWhiteSpace(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ValidationException(new[]
            {
                new ValidationError(field, $"{field} is required.")
            });
        }
    }

    public static void AgainstEmpty(Guid value, string field)
    {
        if (value == Guid.Empty)
        {
            throw new ValidationException(new[]
            {
                new ValidationError(field, $"{field} must not be empty.")
            });
        }
    }

    public static void AgainstNonPositive(decimal value, string field)
    {
        if (value <= 0)
        {
            throw new ValidationException(new[]
            {
                new ValidationError(field, $"{field} must be greater than zero.")
            });
        }
    }
}
