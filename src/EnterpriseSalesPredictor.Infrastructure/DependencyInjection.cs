using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using EnterpriseSalesPredictor.Application.Interfaces;
using EnterpriseSalesPredictor.Application.Interfaces.AccessManagement;
using EnterpriseSalesPredictor.Infrastructure.Security;
using EnterpriseSalesPredictor.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using EnterpriseSalesPredictor.Application.Interfaces.Uploads;
using EnterpriseSalesPredictor.Infrastructure.FileProcessing;
using EnterpriseSalesPredictor.Application.Interfaces.Auditing;
using EnterpriseSalesPredictor.Infrastructure.Auditing;
using EnterpriseSalesPredictor.Application.Interfaces.Sales;
using EnterpriseSalesPredictor.Infrastructure.Sales;
using EnterpriseSalesPredictor.Application.Interfaces.Dashboard;
using EnterpriseSalesPredictor.Infrastructure.Dashboard;
using EnterpriseSalesPredictor.Application.Interfaces.Reports;
using EnterpriseSalesPredictor.Infrastructure.Reports;
using EnterpriseSalesPredictor.Application.Interfaces.Exports;
using EnterpriseSalesPredictor.Infrastructure.Exports;
using EnterpriseSalesPredictor.Application.Interfaces.Forecasting;
using EnterpriseSalesPredictor.Infrastructure.Forecasting;
using EnterpriseSalesPredictor.Application.Interfaces.Replenishment;
using EnterpriseSalesPredictor.Infrastructure.Replenishment;

namespace EnterpriseSalesPredictor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        var connectionString = configuration.GetConnectionString(DatabaseOptions.DefaultConnectionName);
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            services.PostConfigure<DatabaseOptions>(options => options.ConnectionString = connectionString);
        }

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var databaseOptions = serviceProvider
                .GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<DatabaseOptions>>()
                .CurrentValue;

            if (string.IsNullOrWhiteSpace(databaseOptions.ConnectionString))
            {
                return;
            }

            options.UseMySql(
                databaseOptions.ConnectionString,
                new MySqlServerVersion(new Version(8, 0, 36)),
                mysqlOptions =>
                {
                    mysqlOptions.CommandTimeout(databaseOptions.CommandTimeoutSeconds);
                });
        });

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<AuthSeedOptions>(configuration.GetSection(AuthSeedOptions.SectionName));

        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddScoped<ISecurityBootstrapper, SecurityBootstrapper>();
        services.AddScoped<ICredentialValidator, DbCredentialValidator>();
        services.AddScoped<IAccessManagementService, DbAccessManagementService>();
        services.AddScoped<IUploadService, UploadService>();
        services.AddScoped<IUploadProcessingService, UploadProcessingService>();
        services.AddScoped<IUploadFileParser, ExcelUploadFileParser>();
        services.AddScoped<IUploadFileParser, DelimitedUploadFileParser>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<ISalesReadService, SalesReadService>();
        services.AddScoped<IDashboardReadService, DashboardReadService>();
        services.AddScoped<IReportReadService, ReportReadService>();
        services.AddScoped<IExportService, ExcelExportService>();
        services.AddScoped<IForecastService, ForecastService>();
        services.AddScoped<IReplenishmentService, ReplenishmentService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(Repository<>));

        return services;
    }
}
