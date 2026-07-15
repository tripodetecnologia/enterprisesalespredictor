using EnterpriseSalesPredictor.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EnterpriseSalesPredictor.Web.Filters;

public sealed class PermissionFilter : IAsyncActionFilter
{
    private readonly string _permission;

    public PermissionFilter(string permission)
    {
        _permission = permission;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;
        var currentUser = CurrentUserContext.FromClaims(user.Claims);

        if (!currentUser.IsAuthenticated)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", new { returnUrl = context.HttpContext.Request.Path.Value });
            return;
        }

        if (!currentUser.HasPermission(_permission))
        {
            context.Result = new RedirectToActionResult("Forbidden", "Home", null);
            return;
        }

        await next();
    }
}
