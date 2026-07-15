namespace EnterpriseSalesPredictor.Web.ViewModels.Auth;

public sealed class LoginResult
{
    private LoginResult(bool isSuccess, string accessToken, int expiresInMinutes, string? error)
    {
        IsSuccess = isSuccess;
        AccessToken = accessToken;
        ExpiresInMinutes = expiresInMinutes;
        Error = error;
    }

    public bool IsSuccess { get; }

    public string AccessToken { get; }

    public int ExpiresInMinutes { get; }

    public string? Error { get; }

    public static LoginResult Success(string accessToken, int expiresInMinutes)
    {
        return new LoginResult(true, accessToken, expiresInMinutes, null);
    }

    public static LoginResult Failure(string error)
    {
        return new LoginResult(false, string.Empty, 0, error);
    }
}
