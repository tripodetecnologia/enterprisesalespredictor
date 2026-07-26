using System.Security.Claims;

namespace EnterpriseSalesPredictor.Web.Services;

public sealed record CurrentUserContext(
    bool IsAuthenticated,
    string? Name,
    string? Role,
    IReadOnlyCollection<string> Permissions)
{
    public bool HasPermission(string permission)
    {
        return Permissions.Contains(PermissionValues.All) ||
               Permissions.Contains(permission) ||
               Permissions.Any(candidate =>
                   candidate.EndsWith(":*", StringComparison.OrdinalIgnoreCase) &&
                   permission.StartsWith(candidate[..^1], StringComparison.OrdinalIgnoreCase));
    }

    public static CurrentUserContext FromClaims(IEnumerable<Claim> claims)
    {
        var claimList = claims.ToList();
        var permissions = claimList
            .Where(claim => claim.Type == PermissionClaimTypes.Permission)
            .Select(claim => claim.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new CurrentUserContext(
            claimList.Any(claim => claim.Type == ClaimTypes.NameIdentifier),
            claimList.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value,
            claimList.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value,
            permissions);
    }
}
