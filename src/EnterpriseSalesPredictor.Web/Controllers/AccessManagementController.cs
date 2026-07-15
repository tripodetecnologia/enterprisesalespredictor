using EnterpriseSalesPredictor.Web.Filters;
using EnterpriseSalesPredictor.Web.Services;
using EnterpriseSalesPredictor.Web.ViewModels.Access;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Web.Controllers;

[Authorize]
[RequirePermission("users:read")]
public sealed class AccessManagementController : Controller
{
    private readonly AccessManagementApiClient _accessManagementApiClient;

    public AccessManagementController(AccessManagementApiClient accessManagementApiClient)
    {
        _accessManagementApiClient = accessManagementApiClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var viewModel = await BuildPageModelAsync(cancellationToken);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("users:write")]
    public async Task<IActionResult> CreateUser(CreateAccessUserFormViewModel model, CancellationToken cancellationToken)
    {
        model.Permissions = ParsePermissions(model.PermissionsRaw);

        if (!ModelState.IsValid)
        {
            var invalidModel = await BuildPageModelAsync(cancellationToken);
            invalidModel.CreateUserForm = model;
            invalidModel.ErrorMessage = "Please correct the user form fields.";
            return View("Index", invalidModel);
        }

        try
        {
            await _accessManagementApiClient.CreateUserAsync(model, cancellationToken);
            TempData["StatusMessage"] = "User created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception)
        {
            var errorModel = await BuildPageModelAsync(cancellationToken);
            errorModel.CreateUserForm = model;
            errorModel.ErrorMessage = exception.Message;
            return View("Index", errorModel);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("roles:write")]
    public async Task<IActionResult> UpdateRolePermissions(UpdateRolePermissionsFormViewModel model, CancellationToken cancellationToken)
    {
        model.Permissions = ParsePermissions(model.PermissionsRaw);

        if (!ModelState.IsValid)
        {
            var invalidModel = await BuildPageModelAsync(cancellationToken);
            invalidModel.UpdateRoleForm = model;
            invalidModel.ErrorMessage = "Please provide a role to update.";
            return View("Index", invalidModel);
        }

        try
        {
            await _accessManagementApiClient.UpdateRolePermissionsAsync(model, cancellationToken);
            TempData["StatusMessage"] = "Role permissions updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception)
        {
            var errorModel = await BuildPageModelAsync(cancellationToken);
            errorModel.UpdateRoleForm = model;
            errorModel.ErrorMessage = exception.Message;
            return View("Index", errorModel);
        }
    }

    private async Task<AccessManagementPageViewModel> BuildPageModelAsync(CancellationToken cancellationToken)
    {
        var users = await _accessManagementApiClient.GetUsersAsync(cancellationToken);
        var roles = await _accessManagementApiClient.GetRolesAsync(cancellationToken);
        var permissionCatalog = await _accessManagementApiClient.GetPermissionCatalogAsync(cancellationToken);

        return new AccessManagementPageViewModel
        {
            Users = users,
            Roles = roles,
            PermissionCatalog = permissionCatalog,
            StatusMessage = TempData["StatusMessage"] as string
        };
    }

    private static List<string> ParsePermissions(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return new List<string>();
        }

        return raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
