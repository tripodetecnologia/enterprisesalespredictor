using EnterpriseSalesPredictor.Infrastructure.Persistence;
using EnterpriseSalesPredictor.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EnterpriseSalesPredictor.Tests.Integration.Infrastructure;

public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("integration-tests"));

            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            SeedData(dbContext);
        });
    }

    private static void SeedData(AppDbContext dbContext)
    {
        var customer = new Customer(Guid.Parse("10000000-0000-0000-0000-000000000001"), "C-01", "Northwind", "Quito", "North", "Address 1", "555-100");
        var product = new Product(Guid.Parse("20000000-0000-0000-0000-000000000001"), "Hardware", "Industrial Pump", "P-01", "BrandX", 10m, 18m, 6);
        var supplier = new Supplier(Guid.Parse("30000000-0000-0000-0000-000000000001"), "S-01", "Global Supply", "Quito", "Address 2", "555-200");
        var seller = new Seller(Guid.Parse("40000000-0000-0000-0000-000000000001"), "V-01", "Alice");

        dbContext.Customers.Add(customer);
        dbContext.Products.Add(product);
        dbContext.Suppliers.Add(supplier);
        dbContext.Sellers.Add(seller);

        for (var day = 1; day <= 20; day++)
        {
            dbContext.Sales.Add(new Sale(
                Guid.NewGuid(),
                $"INV-{day:000}",
                customer.Id,
                product.Id,
                supplier.Id,
                seller.Id,
                new DateTime(2026, 2, day),
                5m,
                100m + day,
                "Cash"));
        }

        dbContext.SaveChanges();
    }
}
