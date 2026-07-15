using EnterpriseSalesPredictor.Application.Interfaces.AccessManagement;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using EnterpriseSalesPredictor.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EnterpriseSalesPredictor.Tests.Unit.Infrastructure;

public sealed class DbAccessManagementServiceTests
{
    [Test]
    public async Task CreateUserAsync_ShouldPersistUserWithRole()
    {
        await using var dbContext = CreateDbContext();
        var bootstrapper = CreateBootstrapper(dbContext);
        var service = new DbAccessManagementService(dbContext, bootstrapper);

        var result = await service.CreateUserAsync(new CreateAccessUserRequest
        {
            Username = "operator",
            Password = "Operator@123",
            Role = "Operator",
            Permissions = ["sales:read", "reports:read"]
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.Username, Is.EqualTo("operator"));
            Assert.That(result.Role, Is.EqualTo("Operator"));
            Assert.That(result.Permissions, Does.Contain("sales:read"));
        });
    }

    [Test]
    public async Task UpdateRolePermissionsAsync_ShouldReplaceRolePermissions()
    {
        await using var dbContext = CreateDbContext();
        var bootstrapper = CreateBootstrapper(dbContext);
        var service = new DbAccessManagementService(dbContext, bootstrapper);
        await service.CreateUserAsync(new CreateAccessUserRequest
        {
            Username = "viewer",
            Password = "Viewer@123",
            Role = "Viewer",
            Permissions = ["sales:read"]
        });

        var updated = await service.UpdateRolePermissionsAsync(new UpdateRolePermissionsRequest
        {
            Role = "Viewer",
            Permissions = ["dashboard:read", "reports:read"]
        });

        Assert.That(updated.Permissions, Is.EquivalentTo(new[] { "dashboard:read", "reports:read" }));
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new AppDbContext(options);
    }

    private static SecurityBootstrapper CreateBootstrapper(AppDbContext dbContext)
    {
        var options = Options.Create(new AuthSeedOptions());
        return new SecurityBootstrapper(dbContext, new OptionsMonitorStub<AuthSeedOptions>(options.Value));
    }
}
