using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Web.Filters;

public sealed class RequirePermissionAttribute : TypeFilterAttribute
{
    public RequirePermissionAttribute(string permission)
        : base(typeof(PermissionFilter))
    {
        Arguments = new object[] { permission };
    }
}
