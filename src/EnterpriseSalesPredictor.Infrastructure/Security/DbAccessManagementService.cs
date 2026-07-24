using EnterpriseSalesPredictor.Application.Interfaces;
using EnterpriseSalesPredictor.Application.Interfaces.AccessManagement;
using EnterpriseSalesPredictor.Application.Validators;
using EnterpriseSalesPredictor.Domain.Entities;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseSalesPredictor.Infrastructure.Security;

public sealed class DbAccessManagementService : IAccessManagementService
{
    private readonly AppDbContext _dbContext;
    private readonly ISecurityBootstrapper _securityBootstrapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public DbAccessManagementService(AppDbContext dbContext, ISecurityBootstrapper securityBootstrapper, IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _securityBootstrapper = securityBootstrapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyCollection<AccessUserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        await _securityBootstrapper.EnsureSeededAsync(cancellationToken);

        var users = await _dbContext.Users
            .AsNoTracking()
            .Include(entity => entity.UserRoles)
            .ThenInclude(userRole => userRole.Role)
            .ThenInclude(role => role!.RolePermissions)
            .ThenInclude(rolePermission => rolePermission.Permission)
            .OrderBy(entity => entity.Username)
            .ToListAsync(cancellationToken);

        return users.Select(MapUser).ToArray();
    }

    public async Task<AccessUserDto> CreateUserAsync(CreateAccessUserRequest request, CancellationToken cancellationToken = default)
    {
        await _securityBootstrapper.EnsureSeededAsync(cancellationToken);

        Guard.AgainstNullOrWhiteSpace(request.Username, nameof(request.Username));
        Guard.AgainstNullOrWhiteSpace(request.Password, nameof(request.Password));
        Guard.AgainstNullOrWhiteSpace(request.Role, nameof(request.Role));

        var exists = await _dbContext.Users.AnyAsync(entity => entity.Username == request.Username, cancellationToken);
        if (exists)
        {
            throw new ValidationException(new[] { new ValidationError(nameof(request.Username), "Username already exists.") });
        }

        var role = await GetOrCreateRoleAsync(request.Role, cancellationToken);
        if (request.Permissions.Count > 0)
        {
            await UpdateRolePermissionsInternalAsync(role, request.Permissions, cancellationToken);
        }

        var user = new User(Guid.NewGuid(), request.Username.Trim(), string.Empty, true, DateTime.UtcNow);
        user.SetPasswordHash(_passwordHasher.HashPassword(user, request.Password));

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _dbContext.UserRoles.AddAsync(new UserRole(Guid.NewGuid(), user.Id, role.Id), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var loaded = await _dbContext.Users
            .AsNoTracking()
            .Include(entity => entity.UserRoles)
            .ThenInclude(userRole => userRole.Role)
            .ThenInclude(roleEntity => roleEntity!.RolePermissions)
            .ThenInclude(rolePermission => rolePermission.Permission)
            .FirstAsync(entity => entity.Id == user.Id, cancellationToken);

        return MapUser(loaded);
    }

    public async Task<IReadOnlyCollection<RolePermissionsDto>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        await _securityBootstrapper.EnsureSeededAsync(cancellationToken);

        var roles = await _dbContext.Roles
            .AsNoTracking()
            .Include(entity => entity.RolePermissions)
            .ThenInclude(rolePermission => rolePermission.Permission)
            .OrderBy(entity => entity.Name)
            .ToListAsync(cancellationToken);

        return roles.Select(role => new RolePermissionsDto
        {
            Role = role.Name,
            Permissions = role.RolePermissions
                .Select(entity => entity.Permission?.Code)
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .OrderBy(code => code)
                .ToArray()!
        }).ToArray();
    }

    public async Task<RolePermissionsDto> UpdateRolePermissionsAsync(UpdateRolePermissionsRequest request, CancellationToken cancellationToken = default)
    {
        await _securityBootstrapper.EnsureSeededAsync(cancellationToken);
        Guard.AgainstNullOrWhiteSpace(request.Role, nameof(request.Role));

        var role = await _dbContext.Roles.FirstOrDefaultAsync(entity => entity.Name == request.Role, cancellationToken);
        if (role is null)
        {
            throw new ValidationException(new[] { new ValidationError(nameof(request.Role), "Role does not exist.") });
        }

        await UpdateRolePermissionsInternalAsync(role, request.Permissions, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RolePermissionsDto
        {
            Role = role.Name,
            Permissions = request.Permissions
                .Where(permission => !string.IsNullOrWhiteSpace(permission))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(permission => permission)
                .ToArray()
        };
    }

    public Task<IReadOnlyCollection<string>> GetPermissionCatalogAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyCollection<string>>(PermissionCatalog.All);
    }

    private async Task<Role> GetOrCreateRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(entity => entity.Name == roleName.Trim(), cancellationToken);
        if (role is not null)
        {
            return role;
        }

        role = new Role(Guid.NewGuid(), roleName.Trim());
        await _dbContext.Roles.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return role;
    }

    private async Task UpdateRolePermissionsInternalAsync(Role role, IEnumerable<string> requestedPermissions, CancellationToken cancellationToken)
    {
        var permissions = requestedPermissions
            .Where(permission => !string.IsNullOrWhiteSpace(permission))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var invalidPermissions = permissions
            .Where(permission => !PermissionCatalog.All.Contains(permission, StringComparer.OrdinalIgnoreCase))
            .ToArray();

        if (invalidPermissions.Length > 0)
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(requestedPermissions), $"Invalid permissions: {string.Join(", ", invalidPermissions)}")
            });
        }

        var existing = await _dbContext.RolePermissions.Where(entity => entity.RoleId == role.Id).ToListAsync(cancellationToken);
        _dbContext.RolePermissions.RemoveRange(existing);

        var permissionEntities = await _dbContext.Permissions.Where(entity => permissions.Contains(entity.Code)).ToListAsync(cancellationToken);
        foreach (var permission in permissionEntities)
        {
            await _dbContext.RolePermissions.AddAsync(new RolePermission(Guid.NewGuid(), role.Id, permission.Id), cancellationToken);
        }
    }

    private static AccessUserDto MapUser(User user)
    {
        var role = user.UserRoles.Select(entity => entity.Role?.Name).FirstOrDefault(name => !string.IsNullOrWhiteSpace(name)) ?? "User";
        var permissions = user.UserRoles
            .SelectMany(userRole => userRole.Role?.RolePermissions ?? Array.Empty<RolePermission>())
            .Select(rolePermission => rolePermission.Permission?.Code)
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Select(code => code!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(code => code)
            .ToArray();

        return new AccessUserDto
        {
            UserId = user.Id.ToString("N"),
            Username = user.Username,
            Role = role,
            Permissions = permissions
        };
    }
}
