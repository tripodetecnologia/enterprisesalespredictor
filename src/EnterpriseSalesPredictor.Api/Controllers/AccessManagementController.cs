using EnterpriseSalesPredictor.Api.Contracts.Access;
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

    public AccessManagementController(IAccessManagementService accessManagementService)
    {
        _accessManagementService = accessManagementService;
    }

    [HttpGet("users")]
    [Authorize(Policy = "Permission:users:read")]
    public async Task<IActionResult> GetUsersAsync(CancellationToken cancellationToken)
    {
        var users = await _accessManagementService.GetUsersAsync(cancellationToken);
        return Ok(users);
    }

    [HttpPost("users")]
    [Authorize(Policy = "Permission:users:write")]
    public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _accessManagementService.CreateUserAsync(new CreateAccessUserRequestDto
        {
            Username = request.Username,
            Password = request.Password,
            Role = request.Role,
            Permissions = request.Permissions
        }, cancellationToken);

        return CreatedAtAction(nameof(GetUsersAsync), new { id = user.UserId }, user);
    }

    [HttpGet("roles")]
    [Authorize(Policy = "Permission:roles:read")]
    public async Task<IActionResult> GetRolesAsync(CancellationToken cancellationToken)
    {
        var roles = await _accessManagementService.GetRolesAsync(cancellationToken);
        return Ok(roles);
    }

    [HttpPut("roles/permissions")]
    [Authorize(Policy = "Permission:roles:write")]
    public async Task<IActionResult> UpdateRolePermissionsAsync([FromBody] UpdateRolePermissionsRequest request, CancellationToken cancellationToken)
    {
        var role = await _accessManagementService.UpdateRolePermissionsAsync(new UpdateRolePermissionsRequestDto
        {
            Role = request.Role,
            Permissions = request.Permissions
        }, cancellationToken);

        return Ok(role);
    }

    [HttpGet("permissions")]
    [Authorize(Policy = "Permission:roles:read")]
    public async Task<IActionResult> GetPermissionCatalogAsync(CancellationToken cancellationToken)
    {
        var permissions = await _accessManagementService.GetPermissionCatalogAsync(cancellationToken);
        return Ok(permissions);
    }
}
