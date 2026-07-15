namespace EnterpriseSalesPredictor.Infrastructure.Security;

public static class PermissionCatalog
{
    public static readonly string[] All =
    {
        "system:read",
        "security:read",
        "security:write",
        "users:read",
        "users:write",
        "roles:read",
        "roles:write",
        "dashboard:read",
        "reports:read",
        "sales:read",
        "uploads:read",
        "uploads:write",
        "exports:read",
        "exports:write",
        "forecasts:read",
        "forecasts:write",
        "replenishment:read",
        "replenishment:write",
        "audit:read"
    };
}
