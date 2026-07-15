using EnterpriseSalesPredictor.Web.ViewModels.Auth;

namespace EnterpriseSalesPredictor.Web.Services;

public interface IWebAuthApiClient
{
    Task<LoginResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
}
