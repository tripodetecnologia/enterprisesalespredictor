using EnterpriseSalesPredictor.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace EnterpriseSalesPredictor.Infrastructure.Security;

public sealed class ConfiguredUserCredentialValidator : ICredentialValidator
{
    private readonly IOptionsMonitor<AuthSeedOptions> _authSeedOptions;

    public ConfiguredUserCredentialValidator(IOptionsMonitor<AuthSeedOptions> authSeedOptions)
    {
        _authSeedOptions = authSeedOptions;
    }

    public Task<AuthenticatedUser?> ValidateAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var user = _authSeedOptions.CurrentValue.Users.FirstOrDefault(candidate =>
            candidate.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
            candidate.Password == password);

        if (user is null)
        {
            return Task.FromResult<AuthenticatedUser?>(null);
        }

        var authenticatedUser = new AuthenticatedUser(
            user.UserId,
            user.Username,
            user.Role,
            user.Permissions);

        return Task.FromResult<AuthenticatedUser?>(authenticatedUser);
    }
}
