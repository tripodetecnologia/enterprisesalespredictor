namespace EnterpriseSalesPredictor.Application.Interfaces.AccessManagement;

public interface IAccessManagementService
{
    Task<IReadOnlyCollection<AccessUserDto>> GetUsersAsync(CancellationToken cancellationToken = default);

    Task<AccessUserDto> CreateUserAsync(CreateAccessUserRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RolePermissionsDto>> GetRolesAsync(CancellationToken cancellationToken = default);

    Task<RolePermissionsDto> UpdateRolePermissionsAsync(UpdateRolePermissionsRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetPermissionCatalogAsync(CancellationToken cancellationToken = default);
}
