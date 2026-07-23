namespace EnterpriseSalesPredictor.Application.Constants;

public static class PermissionClaimTypes
{
    public const string Permission = "permission";
}

public static class PermissionValues
{
    public const string All = "*";
}

public static class Permissions
{
    public const string SystemRead = "system:read";
    public const string SecurityRead = "security:read";
    public const string SecurityWrite = "security:write";
    public const string UsersRead = "users:read";
    public const string UsersWrite = "users:write";
    public const string RolesRead = "roles:read";
    public const string RolesWrite = "roles:write";
    public const string DashboardRead = "dashboard:read";
    public const string ReportsRead = "reports:read";
    public const string SalesRead = "sales:read";
    public const string UploadsRead = "uploads:read";
    public const string UploadsWrite = "uploads:write";
    public const string ExportsRead = "exports:read";
    public const string ExportsWrite = "exports:write";
    public const string ForecastsRead = "forecasts:read";
    public const string ForecastsWrite = "forecasts:write";
    public const string ReplenishmentRead = "replenishment:read";
    public const string ReplenishmentWrite = "replenishment:write";
    public const string AuditRead = "audit:read";

    public static readonly string[] All =
    {
        SystemRead,
        SecurityRead,
        SecurityWrite,
        UsersRead,
        UsersWrite,
        RolesRead,
        RolesWrite,
        DashboardRead,
        ReportsRead,
        SalesRead,
        UploadsRead,
        UploadsWrite,
        ExportsRead,
        ExportsWrite,
        ForecastsRead,
        ForecastsWrite,
        ReplenishmentRead,
        ReplenishmentWrite,
        AuditRead
    };
}

public static class PermissionPolicies
{
    public const string Prefix = "Permission:";
    public const string UsersRead = Prefix + Permissions.UsersRead;
    public const string UsersWrite = Prefix + Permissions.UsersWrite;
    public const string RolesRead = Prefix + Permissions.RolesRead;
    public const string RolesWrite = Prefix + Permissions.RolesWrite;
    public const string UploadsRead = Prefix + Permissions.UploadsRead;
    public const string UploadsWrite = Prefix + Permissions.UploadsWrite;
    public const string DashboardRead = Prefix + Permissions.DashboardRead;
    public const string ReportsRead = Prefix + Permissions.ReportsRead;
    public const string SalesRead = Prefix + Permissions.SalesRead;
    public const string ExportsRead = Prefix + Permissions.ExportsRead;
    public const string ExportsWrite = Prefix + Permissions.ExportsWrite;
    public const string ForecastsWrite = Prefix + Permissions.ForecastsWrite;
    public const string ReplenishmentRead = Prefix + Permissions.ReplenishmentRead;
    public const string ReplenishmentWrite = Prefix + Permissions.ReplenishmentWrite;
    public const string AuditRead = Prefix + Permissions.AuditRead;
    public const string SystemRead = Prefix + Permissions.SystemRead;
}
