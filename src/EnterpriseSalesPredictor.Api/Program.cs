using EnterpriseSalesPredictor.Application;
using EnterpriseSalesPredictor.Infrastructure;
using EnterpriseSalesPredictor.Api.Middlewares;
using EnterpriseSalesPredictor.Api.Authorization;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using EnterpriseSalesPredictor.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
ApplyDevelopmentConfigurationDefaults(builder);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

ValidateStartupConfiguration(app);

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (dbContext.Database.IsRelational())
    {
        await dbContext.Database.MigrateAsync();
    }
    else
    {
        await dbContext.Database.EnsureCreatedAsync();
    }

    var securityBootstrapper = scope.ServiceProvider.GetRequiredService<ISecurityBootstrapper>();
    await securityBootstrapper.EnsureSeededAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandling();
app.UseCorrelationId();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static void ApplyDevelopmentConfigurationDefaults(WebApplicationBuilder builder)
{
    if (!builder.Environment.IsDevelopment())
    {
        return;
    }

    var signingKeyPath = $"{JwtOptions.SectionName}:SigningKey";
    if (string.IsNullOrWhiteSpace(builder.Configuration[signingKeyPath]))
    {
        builder.Configuration[signingKeyPath] = JwtOptions.DevelopmentSigningKey;
    }
}

static void ValidateStartupConfiguration(WebApplication app)
{
    var jwtOptions = app.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
    if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey) || jwtOptions.SigningKey.Length < JwtOptions.MinimumSigningKeyLength)
    {
        throw new InvalidOperationException($"JWT signing key must be configured with at least {JwtOptions.MinimumSigningKeyLength} characters.");
    }

    if (app.Environment.IsDevelopment())
    {
        return;
    }

    var connectionString = app.Configuration.GetConnectionString(DatabaseOptions.DefaultConnectionName);
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("A database connection string must be configured outside Development.");
    }

    var seedOptions = app.Configuration.GetSection(AuthSeedOptions.SectionName).Get<AuthSeedOptions>() ?? new AuthSeedOptions();
    var unsafeSeedUser = seedOptions.Users.Any(user =>
        user.Permissions.Contains(PermissionValues.All, StringComparer.OrdinalIgnoreCase) ||
        user.Password.Equals("Admin@123", StringComparison.Ordinal));
    if (unsafeSeedUser)
    {
        throw new InvalidOperationException("Default administrator credentials or wildcard seed permissions are not allowed outside Development.");
    }
}

public partial class Program;
