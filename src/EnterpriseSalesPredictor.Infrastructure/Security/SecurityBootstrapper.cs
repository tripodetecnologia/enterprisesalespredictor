using EnterpriseSalesPredictor.Domain.Entities;
using EnterpriseSalesPredictor.Application.Constants;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EnterpriseSalesPredictor.Infrastructure.Security;

public sealed class SecurityBootstrapper : ISecurityBootstrapper
{
    private readonly AppDbContext _dbContext;
    private readonly IOptionsMonitor<AuthSeedOptions> _authSeedOptions;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public SecurityBootstrapper(AppDbContext dbContext, IOptionsMonitor<AuthSeedOptions> authSeedOptions)
    {
        _dbContext = dbContext;
        _authSeedOptions = authSeedOptions;
    }

    public async Task EnsureSeededAsync(CancellationToken cancellationToken = default)
    {
        await EnsurePermissionsAsync(cancellationToken);

        if (await _dbContext.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        foreach (var seedUser in _authSeedOptions.CurrentValue.Users)
        {
            var role = await GetOrCreateRoleAsync(seedUser.Role, cancellationToken);

            var user = new User(Guid.TryParseExact(seedUser.UserId, "N", out var parsedId) ? parsedId : Guid.NewGuid(), seedUser.Username.Trim(), string.Empty, true, DateTime.UtcNow);
            user.SetPasswordHash(_passwordHasher.HashPassword(user, seedUser.Password));

            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await _dbContext.UserRoles.AddAsync(new UserRole(Guid.NewGuid(), user.Id, role.Id), cancellationToken);

            var permissions = seedUser.Permissions.Contains(PermissionValues.All, StringComparer.OrdinalIgnoreCase)
                ? PermissionCatalog.All
                : seedUser.Permissions.ToArray();

            await SetRolePermissionsAsync(role, permissions, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsurePermissionsAsync(CancellationToken cancellationToken)
    {
        var existingCodes = await _dbContext.Permissions
            .Select(entity => entity.Code)
            .ToListAsync(cancellationToken);

        var missingCodes = PermissionCatalog.All
            .Except(existingCodes, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (missingCodes.Length == 0)
        {
            return;
        }

        foreach (var code in missingCodes)
        {
            await _dbContext.Permissions.AddAsync(new Permission(Guid.NewGuid(), code), cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Role> GetOrCreateRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(entity => entity.Name == roleName, cancellationToken);
        if (role is not null)
        {
            return role;
        }

        role = new Role(Guid.NewGuid(), roleName.Trim());
        await _dbContext.Roles.AddAsync(role, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return role;
    }

    private async Task SetRolePermissionsAsync(Role role, IEnumerable<string> codes, CancellationToken cancellationToken)
    {
        var codeSet = codes
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var permissions = await _dbContext.Permissions
            .Where(entity => codeSet.Contains(entity.Code))
            .ToListAsync(cancellationToken);

        var existing = await _dbContext.RolePermissions.Where(entity => entity.RoleId == role.Id).ToListAsync(cancellationToken);
        _dbContext.RolePermissions.RemoveRange(existing);

        foreach (var permission in permissions)
        {
            await _dbContext.RolePermissions.AddAsync(new RolePermission(Guid.NewGuid(), role.Id, permission.Id), cancellationToken);
        }
    }
}
