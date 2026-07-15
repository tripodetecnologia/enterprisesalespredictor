using ClosedXML.Excel;
using EnterpriseSalesPredictor.Application.Interfaces.Exports;
using EnterpriseSalesPredictor.Application.Interfaces.Reports;
using EnterpriseSalesPredictor.Application.Interfaces.Sales;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseSalesPredictor.Infrastructure.Exports;

public sealed class ExcelExportService : IExportService
{
    private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private readonly AppDbContext _dbContext;
    private readonly IReportReadService _reportReadService;
    private readonly ISalesReadService _salesReadService;

    public ExcelExportService(
        AppDbContext dbContext,
        IReportReadService reportReadService,
        ISalesReadService salesReadService)
    {
        _dbContext = dbContext;
        _reportReadService = reportReadService;
        _salesReadService = salesReadService;
    }

    public async Task<ExportFileDto> ExportReportsAsync(ReportQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook();

        var reports = new[]
        {
            await _reportReadService.GetManagementReportAsync(criteria, cancellationToken),
            await _reportReadService.GetCommercialReportAsync(criteria, cancellationToken),
            await _reportReadService.GetOperationalReportAsync(criteria, cancellationToken),
            await _reportReadService.GetReplenishmentReportAsync(criteria, cancellationToken),
            await _reportReadService.GetPredictiveReportAsync(criteria, cancellationToken)
        };

        foreach (var report in reports)
        {
            var worksheet = workbook.Worksheets.Add(SanitizeSheetName(report.Title));
            worksheet.Cell(1, 1).Value = report.Title;
            worksheet.Cell(2, 1).Value = "GeneratedAtUtc";
            worksheet.Cell(2, 2).Value = report.GeneratedAtUtc;

            var row = 4;
            foreach (var section in report.Sections)
            {
                worksheet.Cell(row, 1).Value = section.Title;
                row++;
                worksheet.Cell(row, 1).Value = "Metric";
                worksheet.Cell(row, 2).Value = "Value";
                row++;

                foreach (var metric in section.Metrics)
                {
                    worksheet.Cell(row, 1).Value = metric.Label;
                    worksheet.Cell(row, 2).Value = metric.Value;
                    row++;
                }

                row++;
            }

            worksheet.Columns().AdjustToContents();
        }

        return CreateFile($"reports-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx", workbook);
    }

    public async Task<ExportFileDto> ExportFilteredSalesAsync(SalesQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("FilteredSales");
        var sales = await _salesReadService.QuerySalesAsync(criteria, cancellationToken);

        WriteSalesHeader(worksheet);
        var row = 2;
        foreach (var sale in sales)
        {
            worksheet.Cell(row, 1).Value = sale.Id.ToString();
            worksheet.Cell(row, 2).Value = sale.InvoiceNumber;
            worksheet.Cell(row, 3).Value = sale.CustomerId.ToString();
            worksheet.Cell(row, 4).Value = sale.ProductId.ToString();
            worksheet.Cell(row, 5).Value = sale.SupplierId.ToString();
            worksheet.Cell(row, 6).Value = sale.SellerId.ToString();
            worksheet.Cell(row, 7).Value = sale.SaleDate;
            worksheet.Cell(row, 8).Value = sale.Quantity;
            worksheet.Cell(row, 9).Value = sale.SaleAmount;
            worksheet.Cell(row, 10).Value = sale.PaymentMethod;
            row++;
        }

        worksheet.Columns().AdjustToContents();
        return CreateFile($"sales-filtered-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx", workbook);
    }

    public async Task<ExportFileDto> ExportBaseDataAsync(CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook();

        var customersSheet = workbook.Worksheets.Add("Customers");
        customersSheet.Cell(1, 1).Value = "Id";
        customersSheet.Cell(1, 2).Value = "Identification";
        customersSheet.Cell(1, 3).Value = "Name";
        customersSheet.Cell(1, 4).Value = "City";
        customersSheet.Cell(1, 5).Value = "Zone";
        var customerRow = 2;
        foreach (var customer in await _dbContext.Customers.AsNoTracking().ToListAsync(cancellationToken))
        {
            customersSheet.Cell(customerRow, 1).Value = customer.Id.ToString();
            customersSheet.Cell(customerRow, 2).Value = customer.Identification;
            customersSheet.Cell(customerRow, 3).Value = customer.Name;
            customersSheet.Cell(customerRow, 4).Value = customer.City;
            customersSheet.Cell(customerRow, 5).Value = customer.Zone;
            customerRow++;
        }

        var productsSheet = workbook.Worksheets.Add("Products");
        productsSheet.Cell(1, 1).Value = "Id";
        productsSheet.Cell(1, 2).Value = "Type";
        productsSheet.Cell(1, 3).Value = "Name";
        productsSheet.Cell(1, 4).Value = "Reference";
        productsSheet.Cell(1, 5).Value = "Brand";
        productsSheet.Cell(1, 6).Value = "AvailableUnits";
        var productRow = 2;
        foreach (var product in await _dbContext.Products.AsNoTracking().ToListAsync(cancellationToken))
        {
            productsSheet.Cell(productRow, 1).Value = product.Id.ToString();
            productsSheet.Cell(productRow, 2).Value = product.ProductType;
            productsSheet.Cell(productRow, 3).Value = product.Name;
            productsSheet.Cell(productRow, 4).Value = product.Reference;
            productsSheet.Cell(productRow, 5).Value = product.Brand;
            productsSheet.Cell(productRow, 6).Value = product.AvailableUnits;
            productRow++;
        }

        var salesSheet = workbook.Worksheets.Add("Sales");
        WriteSalesHeader(salesSheet);
        var saleRow = 2;
        foreach (var sale in await _dbContext.Sales.AsNoTracking().OrderByDescending(item => item.SaleDate).ToListAsync(cancellationToken))
        {
            salesSheet.Cell(saleRow, 1).Value = sale.Id.ToString();
            salesSheet.Cell(saleRow, 2).Value = sale.InvoiceNumber;
            salesSheet.Cell(saleRow, 3).Value = sale.CustomerId.ToString();
            salesSheet.Cell(saleRow, 4).Value = sale.ProductId.ToString();
            salesSheet.Cell(saleRow, 5).Value = sale.SupplierId.ToString();
            salesSheet.Cell(saleRow, 6).Value = sale.SellerId.ToString();
            salesSheet.Cell(saleRow, 7).Value = sale.SaleDate;
            salesSheet.Cell(saleRow, 8).Value = sale.Quantity;
            salesSheet.Cell(saleRow, 9).Value = sale.SaleAmount;
            salesSheet.Cell(saleRow, 10).Value = sale.PaymentMethod;
            saleRow++;
        }

        foreach (var worksheet in workbook.Worksheets)
        {
            worksheet.Columns().AdjustToContents();
        }

        return CreateFile($"base-data-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx", workbook);
    }

    private static void WriteSalesHeader(IXLWorksheet worksheet)
    {
        worksheet.Cell(1, 1).Value = "Id";
        worksheet.Cell(1, 2).Value = "InvoiceNumber";
        worksheet.Cell(1, 3).Value = "CustomerId";
        worksheet.Cell(1, 4).Value = "ProductId";
        worksheet.Cell(1, 5).Value = "SupplierId";
        worksheet.Cell(1, 6).Value = "SellerId";
        worksheet.Cell(1, 7).Value = "SaleDate";
        worksheet.Cell(1, 8).Value = "Quantity";
        worksheet.Cell(1, 9).Value = "SaleAmount";
        worksheet.Cell(1, 10).Value = "PaymentMethod";
    }

    private static string SanitizeSheetName(string title)
    {
        var invalidChars = Path.GetInvalidFileNameChars().Concat(new[] { '[', ']', '*', '?', '/', '\\' }).ToHashSet();
        var sanitized = new string(title.Where(character => !invalidChars.Contains(character)).ToArray());
        return string.IsNullOrWhiteSpace(sanitized) ? "Report" : sanitized[..Math.Min(31, sanitized.Length)];
    }

    private static ExportFileDto CreateFile(string fileName, XLWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return new ExportFileDto
        {
            FileName = fileName,
            ContentType = ExcelContentType,
            Content = stream.ToArray()
        };
    }
}
