using EnterpriseSalesPredictor.Application.Constants;
using Microsoft.AspNetCore.Authorization;

namespace EnterpriseSalesPredictor.Api.Authorization;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var permissions = context.User.FindAll(PermissionClaimTypes.Permission).Select(claim => claim.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (permissions.Contains(requirement.Permission) || permissions.Contains(PermissionValues.All))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var lastSeparator = requirement.Permission.LastIndexOf(':');
        if (lastSeparator > 0)
        {
            var moduleWildcard = $"{requirement.Permission[..lastSeparator]}:*";
            if (permissions.Contains(moduleWildcard))
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
