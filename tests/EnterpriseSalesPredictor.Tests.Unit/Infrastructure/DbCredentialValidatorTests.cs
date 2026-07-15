using EnterpriseSalesPredictor.Infrastructure.Persistence;
using EnterpriseSalesPredictor.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EnterpriseSalesPredictor.Tests.Unit.Infrastructure;

public sealed class DbCredentialValidatorTests
{
    [Test]
    public async Task ValidateAsync_ShouldAuthenticateSeededUser()
    {
        await using var dbContext = CreateDbContext();
        var bootstrapper = CreateBootstrapper(dbContext);
        await bootstrapper.EnsureSeededAsync();
        var validator = new DbCredentialValidator(dbContext, bootstrapper);

        var user = await validator.ValidateAsync("seedadmin", "SeedAdmin@123");

        Assert.Multiple(() =>
        {
            Assert.That(user, Is.Not.Null);
            Assert.That(user!.Username, Is.EqualTo("seedadmin"));
            Assert.That(user.Permissions, Is.Not.Empty);
        });
    }

    [Test]
    public async Task ValidateAsync_ShouldRejectInvalidPassword()
    {
        await using var dbContext = CreateDbContext();
        var bootstrapper = CreateBootstrapper(dbContext);
        await bootstrapper.EnsureSeededAsync();
        var validator = new DbCredentialValidator(dbContext, bootstrapper);

        var user = await validator.ValidateAsync("seedadmin", "wrong-password");

        Assert.That(user, Is.Null);
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
        var options = Options.Create(new AuthSeedOptions
        {
            Users =
            [
                new AuthSeedUser
                {
                    UserId = Guid.NewGuid().ToString("N"),
                    Username = "seedadmin",
                    Password = "SeedAdmin@123",
                    Role = "Administrator",
                    Permissions = PermissionCatalog.All.ToList()
                }
            ]
        });

        return new SecurityBootstrapper(dbContext, new OptionsMonitorStub<AuthSeedOptions>(options.Value));
    }
}
