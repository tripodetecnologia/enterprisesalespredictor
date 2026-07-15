using EnterpriseSalesPredictor.Application.Interfaces;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseSalesPredictor.Infrastructure.Security;

public sealed class DbCredentialValidator : ICredentialValidator
{
    private readonly AppDbContext _dbContext;
    private readonly ISecurityBootstrapper _securityBootstrapper;
    private readonly PasswordHasher<Domain.Entities.User> _passwordHasher = new();

    public DbCredentialValidator(AppDbContext dbContext, ISecurityBootstrapper securityBootstrapper)
    {
        _dbContext = dbContext;
        _securityBootstrapper = securityBootstrapper;
    }

    public async Task<AuthenticatedUser?> ValidateAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        await _securityBootstrapper.EnsureSeededAsync(cancellationToken);

        var user = await _dbContext.Users
            .Include(entity => entity.UserRoles)
            .ThenInclude(userRole => userRole.Role)
            .ThenInclude(role => role!.RolePermissions)
            .ThenInclude(rolePermission => rolePermission.Permission)
            .FirstOrDefaultAsync(entity => entity.Username == username && entity.IsActive, cancellationToken);

        if (user is null)
        {
            return null;
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        var primaryRole = user.UserRoles
            .Select(userRole => userRole.Role)
            .Where(role => role is not null)
            .Select(role => role!.Name)
            .FirstOrDefault() ?? "User";

        var permissions = user.UserRoles
            .SelectMany(userRole => userRole.Role?.RolePermissions ?? Array.Empty<Domain.Entities.RolePermission>())
            .Select(rolePermission => rolePermission.Permission?.Code)
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Select(code => code!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new AuthenticatedUser(user.Id.ToString("N"), user.Username, primaryRole, permissions);
    }
}
