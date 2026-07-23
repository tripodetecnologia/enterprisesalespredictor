using EnterpriseSalesPredictor.Application.Constants;
using EnterpriseSalesPredictor.Application.Interfaces.AccessManagement;
using EnterpriseSalesPredictor.Application.Validators;
using Microsoft.Extensions.Options;

namespace EnterpriseSalesPredictor.Infrastructure.Security;

public sealed class InMemoryAccessManagementService : IAccessManagementService
{
    private readonly IOptionsMonitor<AuthSeedOptions> _authSeedOptions;

    public InMemoryAccessManagementService(IOptionsMonitor<AuthSeedOptions> authSeedOptions)
    {
        _authSeedOptions = authSeedOptions;
    }

    public Task<IReadOnlyCollection<AccessUserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = _authSeedOptions.CurrentValue.Users
            .Select(MapUser)
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<AccessUserDto>>(users);
    }

    public Task<AccessUserDto> CreateUserAsync(CreateAccessUserRequest request, CancellationToken cancellationToken = default)
    {
        Guard.AgainstNullOrWhiteSpace(request.Username, nameof(request.Username));
        Guard.AgainstNullOrWhiteSpace(request.Password, nameof(request.Password));
        Guard.AgainstNullOrWhiteSpace(request.Role, nameof(request.Role));

        var existing = _authSeedOptions.CurrentValue.Users.Any(user =>
            user.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase));
        if (existing)
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(request.Username), "Username already exists.")
            });
        }

        var validatedPermissions = request.Permissions
            .Where(permission => !string.IsNullOrWhiteSpace(permission))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var invalidPermissions = validatedPermissions
            .Where(permission => !PermissionCatalog.All.Contains(permission, StringComparer.OrdinalIgnoreCase) && permission != PermissionValues.All)
            .ToArray();

        if (invalidPermissions.Length > 0)
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(request.Permissions), $"Invalid permissions: {string.Join(", ", invalidPermissions)}")
            });
        }

        var user = new AuthSeedUser
        {
            UserId = Guid.NewGuid().ToString("N"),
            Username = request.Username.Trim(),
            Password = request.Password,
            Role = request.Role.Trim(),
            Permissions = validatedPermissions.ToList()
        };

        _authSeedOptions.CurrentValue.Users.Add(user);
        return Task.FromResult(MapUser(user));
    }

    public Task<IReadOnlyCollection<RolePermissionsDto>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = _authSeedOptions.CurrentValue.Users
            .GroupBy(user => user.Role, StringComparer.OrdinalIgnoreCase)
            .Select(group => new RolePermissionsDto
            {
                Role = group.Key,
                Permissions = group
                    .SelectMany(user => user.Permissions)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(permission => permission)
                    .ToArray()
            })
            .OrderBy(role => role.Role)
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<RolePermissionsDto>>(roles);
    }

    public Task<RolePermissionsDto> UpdateRolePermissionsAsync(UpdateRolePermissionsRequest request, CancellationToken cancellationToken = default)
    {
        Guard.AgainstNullOrWhiteSpace(request.Role, nameof(request.Role));

        var matchingUsers = _authSeedOptions.CurrentValue.Users
            .Where(user => user.Role.Equals(request.Role, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (matchingUsers.Length == 0)
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(request.Role), "Role does not exist.")
            });
        }

        var permissions = request.Permissions
            .Where(permission => !string.IsNullOrWhiteSpace(permission))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var invalidPermissions = permissions
            .Where(permission => !PermissionCatalog.All.Contains(permission, StringComparer.OrdinalIgnoreCase) && permission != PermissionValues.All)
            .ToArray();

        if (invalidPermissions.Length > 0)
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(request.Permissions), $"Invalid permissions: {string.Join(", ", invalidPermissions)}")
            });
        }

        foreach (var user in matchingUsers)
        {
            user.Permissions = permissions.ToList();
        }

        return Task.FromResult(new RolePermissionsDto
        {
            Role = matchingUsers[0].Role,
            Permissions = permissions
        });
    }

    public Task<IReadOnlyCollection<string>> GetPermissionCatalogAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyCollection<string>>(PermissionCatalog.All);
    }

    private static AccessUserDto MapUser(AuthSeedUser user)
    {
        return new AccessUserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Role = user.Role,
            Permissions = user.Permissions
        };
    }
}
