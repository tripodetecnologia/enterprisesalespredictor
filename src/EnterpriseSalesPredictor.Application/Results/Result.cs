namespace EnterpriseSalesPredictor.Application.Results;

public class Result
{
    protected Result(bool isSuccess, string? errorCode, string? errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public string? ErrorCode { get; }

    public string? ErrorMessage { get; }

    public static Result Success()
    {
        return new Result(true, null, null);
    }

    public static Result Failure(string errorCode, string errorMessage)
    {
        return new Result(false, errorCode, errorMessage);
    }
}

public sealed class Result<TValue> : Result
{
    private Result(bool isSuccess, TValue? value, string? errorCode, string? errorMessage)
        : base(isSuccess, errorCode, errorMessage)
    {
        Value = value;
    }

    public TValue? Value { get; }

    public static Result<TValue> Success(TValue value)
    {
        return new Result<TValue>(true, value, null, null);
    }

    public static new Result<TValue> Failure(string errorCode, string errorMessage)
    {
        return new Result<TValue>(false, default, errorCode, errorMessage);
    }
}
