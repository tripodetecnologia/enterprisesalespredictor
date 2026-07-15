namespace EnterpriseSalesPredictor.Application.Validators;

public sealed class ValidationException : Exception
{
    public ValidationException(IEnumerable<ValidationError> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors.ToArray();
    }

    public IReadOnlyCollection<ValidationError> Errors { get; }
}
