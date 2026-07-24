using EnterpriseSalesPredictor.Api.Contracts.Access;
using EnterpriseSalesPredictor.Application.Interfaces.Auditing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAccessManagementService = EnterpriseSalesPredictor.Application.Interfaces.AccessManagement.IAccessManagementService;
using CreateAccessUserRequestDto = EnterpriseSalesPredictor.Application.Interfaces.AccessManagement.CreateAccessUserRequest;
using UpdateRolePermissionsRequestDto = EnterpriseSalesPredictor.Application.Interfaces.AccessManagement.UpdateRolePermissionsRequest;

namespace EnterpriseSalesPredictor.Api.Controllers;

[ApiController]
[Route("api/access")]
[Authorize]
public sealed class AccessManagementController : ControllerBase
{
    private readonly IAccessManagementService _accessManagementService;
    private readonly IAuditLogService _auditLogService;

    public AccessManagementController(IAccessManagementService accessManagementService, IAuditLogService auditLogService)
    {
        _accessManagementService = accessManagementService;
        _auditLogService = auditLogService;
    }

    [HttpGet("users")]
    [Authorize(Policy = PermissionPolicies.UsersRead)]
    public async Task<IActionResult> GetUsersAsync(CancellationToken cancellationToken)
    {
        var users = await _accessManagementService.GetUsersAsync(cancellationToken);
        return Ok(users);
    }

    [HttpPost("users")]
    [Authorize(Policy = PermissionPolicies.UsersWrite)]
    public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _accessManagementService.CreateUserAsync(new CreateAccessUserRequestDto
        {
            Username = request.Username,
            Password = request.Password,
            Role = request.Role,
            Permissions = request.Permissions
        }, cancellationToken);

        await _auditLogService.RecordAsync(new CreateAuditLogCommand
        {
            Actor = GetActor(),
            Action = "UserCreated",
            Module = "AccessManagement",
            Details = $"Username={user.Username}; Role={user.Role}; Permissions={string.Join(',', user.Permissions)}"
        }, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, user);
    }

    [HttpGet("roles")]
    [Authorize(Policy = PermissionPolicies.RolesRead)]
    public async Task<IActionResult> GetRolesAsync(CancellationToken cancellationToken)
    {
        var roles = await _accessManagementService.GetRolesAsync(cancellationToken);
        return Ok(roles);
    }

    [HttpPut("roles/permissions")]
    [Authorize(Policy = PermissionPolicies.RolesWrite)]
    public async Task<IActionResult> UpdateRolePermissionsAsync([FromBody] UpdateRolePermissionsRequest request, CancellationToken cancellationToken)
    {
        var previousRole = (await _accessManagementService.GetRolesAsync(cancellationToken))
            .FirstOrDefault(item => item.Role.Equals(request.Role, StringComparison.OrdinalIgnoreCase));

        var role = await _accessManagementService.UpdateRolePermissionsAsync(new UpdateRolePermissionsRequestDto
        {
            Role = request.Role,
            Permissions = request.Permissions
        }, cancellationToken);

        await _auditLogService.RecordAsync(new CreateAuditLogCommand
        {
            Actor = GetActor(),
            Action = "RolePermissionsUpdated",
            Module = "AccessManagement",
            Details = $"Role={role.Role}; PreviousPermissions={string.Join(',', previousRole?.Permissions ?? Array.Empty<string>())}; NewPermissions={string.Join(',', role.Permissions)}"
        }, cancellationToken);

        return Ok(role);
    }

    [HttpGet("permissions")]
    [Authorize(Policy = PermissionPolicies.RolesRead)]
    public async Task<IActionResult> GetPermissionCatalogAsync(CancellationToken cancellationToken)
    {
        var permissions = await _accessManagementService.GetPermissionCatalogAsync(cancellationToken);
        return Ok(permissions);
    }

    private string GetActor()
    {
        return User.Identity?.Name ?? "system";
    }
}
