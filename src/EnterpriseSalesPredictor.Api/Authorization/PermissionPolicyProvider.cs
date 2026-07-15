using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace EnterpriseSalesPredictor.Api.Authorization;

public sealed class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private const string PolicyPrefix = "Permission:";

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(PolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return base.GetPolicyAsync(policyName);
        }

        var permission = policyName[PolicyPrefix.Length..];
        var policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(permission))
            .RequireAuthenticatedUser()
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
}
