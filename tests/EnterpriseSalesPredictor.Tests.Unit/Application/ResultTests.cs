using EnterpriseSalesPredictor.Application.Results;

namespace EnterpriseSalesPredictor.Tests.Unit.Application;

public sealed class ResultTests
{
    [Test]
    public void Success_ShouldCreateSuccessfulResult()
    {
        var result = Result.Success();

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.ErrorCode, Is.Null);
            Assert.That(result.ErrorMessage, Is.Null);
        });
    }

    [Test]
    public void Failure_ShouldCreateFailedResultWithError()
    {
        var result = Result.Failure("validation", "Invalid payload");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.ErrorCode, Is.EqualTo("validation"));
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid payload"));
        });
    }

    [Test]
    public void GenericSuccess_ShouldExposeValue()
    {
        var result = Result<int>.Success(42);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(42));
        });
    }

    [Test]
    public void GenericFailure_ShouldClearValue()
    {
        var result = Result<int>.Failure("error", "Failed");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Value, Is.EqualTo(default(int)));
            Assert.That(result.ErrorCode, Is.EqualTo("error"));
        });
    }
}
