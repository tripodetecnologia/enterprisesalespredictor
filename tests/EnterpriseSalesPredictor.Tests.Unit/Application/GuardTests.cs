using EnterpriseSalesPredictor.Application.Validators;

namespace EnterpriseSalesPredictor.Tests.Unit.Application;

public sealed class GuardTests
{
    [Test]
    public void AgainstNullOrWhiteSpace_ShouldThrowValidationException()
    {
        var exception = Assert.Throws<ValidationException>(() => Guard.AgainstNullOrWhiteSpace(" ", "Name"));

        Assert.That(exception!.Errors.Single(), Is.EqualTo(new ValidationError("Name", "Name is required.")));
    }

    [Test]
    public void AgainstEmpty_ShouldThrowValidationException()
    {
        var exception = Assert.Throws<ValidationException>(() => Guard.AgainstEmpty(Guid.Empty, "Id"));

        Assert.That(exception!.Errors.Single(), Is.EqualTo(new ValidationError("Id", "Id must not be empty.")));
    }

    [Test]
    public void AgainstNonPositive_ShouldThrowValidationException()
    {
        var exception = Assert.Throws<ValidationException>(() => Guard.AgainstNonPositive(0m, "Amount"));

        Assert.That(exception!.Errors.Single(), Is.EqualTo(new ValidationError("Amount", "Amount must be greater than zero.")));
    }

    [Test]
    public void ValidValues_ShouldNotThrow()
    {
        Assert.DoesNotThrow(() =>
        {
            Guard.AgainstNullOrWhiteSpace("ok", "Name");
            Guard.AgainstEmpty(Guid.NewGuid(), "Id");
            Guard.AgainstNonPositive(12m, "Amount");
        });
    }
}
