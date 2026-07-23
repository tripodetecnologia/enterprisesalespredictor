using EnterpriseSalesPredictor.Application.DTOs.Dashboard;
using EnterpriseSalesPredictor.Application.Interfaces.Dashboard;
using EnterpriseSalesPredictor.Domain.Entities;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseSalesPredictor.Infrastructure.Dashboard;

public sealed class DashboardReadService : IDashboardReadService
{
    private readonly AppDbContext _dbContext;

    public DashboardReadService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardKpiDto> GetKpisAsync(DashboardQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        var sales = ApplyDateFilters(_dbContext.Sales.AsNoTracking(), criteria);
        var totalTransactions = await sales.CountAsync(cancellationToken);
        var totalSalesAmount = totalTransactions == 0 ? 0m : await sales.SumAsync(sale => sale.SaleAmount, cancellationToken);
        var totalQuantity = totalTransactions == 0 ? 0m : await sales.SumAsync(sale => sale.Quantity, cancellationToken);

        return new DashboardKpiDto
        {
            TotalSalesAmount = totalSalesAmount,
            TotalQuantity = totalQuantity,
            TotalTransactions = totalTransactions,
            AverageTicket = totalTransactions == 0 ? 0m : totalSalesAmount / totalTransactions
        };
    }

    public async Task<IReadOnlyCollection<DashboardBreakdownItemDto>> GetTopCustomersAsync(DashboardQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        var limit = NormalizeLimit(criteria.Limit);
        var sales = ApplyDateFilters(_dbContext.Sales.AsNoTracking(), criteria);

        return await sales
            .Join(_dbContext.Customers.AsNoTracking(), sale => sale.CustomerId, customer => customer.Id, (sale, customer) => new { sale, customer })
            .GroupBy(item => item.customer.Name)
            .Select(group => new DashboardBreakdownItemDto
            {
                Label = group.Key,
                TotalSalesAmount = group.Sum(item => item.sale.SaleAmount),
                TotalQuantity = group.Sum(item => item.sale.Quantity),
                TotalTransactions = group.Count()
            })
            .OrderByDescending(item => item.TotalSalesAmount)
            .Take(limit)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<DashboardBreakdownItemDto>> GetTopProductsAsync(DashboardQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        var limit = NormalizeLimit(criteria.Limit);
        var sales = ApplyDateFilters(_dbContext.Sales.AsNoTracking(), criteria);

        return await sales
            .Join(_dbContext.Products.AsNoTracking(), sale => sale.ProductId, product => product.Id, (sale, product) => new { sale, product })
            .GroupBy(item => item.product.Name)
            .Select(group => new DashboardBreakdownItemDto
            {
                Label = group.Key,
                TotalSalesAmount = group.Sum(item => item.sale.SaleAmount),
                TotalQuantity = group.Sum(item => item.sale.Quantity),
                TotalTransactions = group.Count()
            })
            .OrderByDescending(item => item.TotalSalesAmount)
            .Take(limit)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<DashboardBreakdownItemDto>> GetSalesByProductLineAsync(DashboardQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        var sales = ApplyDateFilters(_dbContext.Sales.AsNoTracking(), criteria);

        return await sales
            .Join(_dbContext.Products.AsNoTracking(), sale => sale.ProductId, product => product.Id, (sale, product) => new { sale, product })
            .GroupBy(item => item.product.ProductType)
            .Select(group => new DashboardBreakdownItemDto
            {
                Label = group.Key,
                TotalSalesAmount = group.Sum(item => item.sale.SaleAmount),
                TotalQuantity = group.Sum(item => item.sale.Quantity),
                TotalTransactions = group.Count()
            })
            .OrderByDescending(item => item.TotalSalesAmount)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<DashboardBreakdownItemDto>> GetSalesBySupplierAsync(DashboardQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        var sales = ApplyDateFilters(_dbContext.Sales.AsNoTracking(), criteria);

        return await sales
            .Join(_dbContext.Suppliers.AsNoTracking(), sale => sale.SupplierId, supplier => supplier.Id, (sale, supplier) => new { sale, supplier })
            .GroupBy(item => item.supplier.Name)
            .Select(group => new DashboardBreakdownItemDto
            {
                Label = group.Key,
                TotalSalesAmount = group.Sum(item => item.sale.SaleAmount),
                TotalQuantity = group.Sum(item => item.sale.Quantity),
                TotalTransactions = group.Count()
            })
            .OrderByDescending(item => item.TotalSalesAmount)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<DashboardAlertDto>> GetCommercialAlertsAsync(DashboardQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        var alerts = new List<DashboardAlertDto>();
        var sales = ApplyDateFilters(_dbContext.Sales.AsNoTracking(), criteria);
        var totalTransactions = await sales.CountAsync(cancellationToken);

        if (totalTransactions == 0)
        {
            alerts.Add(new DashboardAlertDto
            {
                Severity = "warning",
                Title = "Sin ventas en el período",
                Message = "El periodo seleccionado no tiene transacciones de venta para alimentar el panel."
            });
            return alerts;
        }

        var lowStockProducts = await _dbContext.Products.AsNoTracking()
            .Where(product => product.AvailableUnits <= 5)
            .OrderBy(product => product.AvailableUnits)
            .Take(3)
            .ToListAsync(cancellationToken);

        foreach (var product in lowStockProducts)
        {
            alerts.Add(new DashboardAlertDto
            {
                Severity = "critical",
                Title = "Stock bajo detectado",
                Message = $"El producto '{product.Name}' solo tiene {product.AvailableUnits} unidades disponibles."
            });
        }

        var topCustomer = await sales
            .Join(_dbContext.Customers.AsNoTracking(), sale => sale.CustomerId, customer => customer.Id, (sale, customer) => new { sale, customer })
            .GroupBy(item => item.customer.Name)
            .Select(group => new
            {
                Customer = group.Key,
                Amount = group.Sum(item => item.sale.SaleAmount)
            })
            .OrderByDescending(item => item.Amount)
            .FirstOrDefaultAsync(cancellationToken);

        var totalAmount = await sales.SumAsync(sale => sale.SaleAmount, cancellationToken);
        if (topCustomer is not null && totalAmount > 0m)
        {
            var share = topCustomer.Amount / totalAmount;
            if (share >= 0.5m)
            {
                alerts.Add(new DashboardAlertDto
                {
                    Severity = "warning",
                    Title = "Concentración de ingresos",
                    Message = $"El cliente '{topCustomer.Customer}' representa el {share:P0} de las ventas en el periodo seleccionado."
                });
            }
        }

        var today = DateTime.UtcNow.Date;
        var previousStart = today.AddDays(-14);
        var recentStart = today.AddDays(-7);

        var previousWeekSales = await _dbContext.Sales.AsNoTracking()
            .Where(sale => sale.SaleDate >= previousStart && sale.SaleDate < recentStart)
            .SumAsync(sale => (decimal?)sale.SaleAmount, cancellationToken) ?? 0m;

        var recentWeekSales = await _dbContext.Sales.AsNoTracking()
            .Where(sale => sale.SaleDate >= recentStart && sale.SaleDate < today.AddDays(1))
            .SumAsync(sale => (decimal?)sale.SaleAmount, cancellationToken) ?? 0m;

        if (previousWeekSales > 0m && recentWeekSales < previousWeekSales * 0.8m)
        {
            alerts.Add(new DashboardAlertDto
            {
                Severity = "warning",
                Title = "Desaceleración semanal de ventas",
                Message = $"Las ventas de los últimos 7 días ({recentWeekSales:N0}) están más de un 20% por debajo del periodo anterior de 7 días ({previousWeekSales:N0})."
            });
        }

        return alerts;
    }

    private static IQueryable<Sale> ApplyDateFilters(IQueryable<Sale> query, DashboardQueryCriteria criteria)
    {
        if (criteria.FromDate.HasValue)
        {
            query = query.Where(sale => sale.SaleDate >= criteria.FromDate.Value);
        }

        if (criteria.ToDate.HasValue)
        {
            query = query.Where(sale => sale.SaleDate <= criteria.ToDate.Value);
        }

        return query;
    }

    private static int NormalizeLimit(int limit)
    {
        if (limit <= 0)
        {
            return 5;
        }

        return Math.Min(limit, 20);
    }
}
