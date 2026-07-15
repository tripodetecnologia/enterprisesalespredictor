using ClosedXML.Excel;
using EnterpriseSalesPredictor.Application.Interfaces.Uploads;

namespace EnterpriseSalesPredictor.Infrastructure.FileProcessing;

public sealed class ExcelUploadFileParser : IUploadFileParser
{
    public string ParserKey => "excel";

    public bool CanHandle(string fileName)
    {
        return fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) ||
               fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase);
    }

    public Task<UploadParseResult> ParseAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var result = new UploadParseResult();
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.FirstOrDefault();
        if (worksheet is null)
        {
            result.Errors.Add(new UploadParseError
            {
                RowNumber = 0,
                FieldName = "worksheet",
                ErrorMessage = "Workbook does not contain worksheets."
            });
            return Task.FromResult(result);
        }

        var usedRange = worksheet.RangeUsed();
        if (usedRange is null)
        {
            result.Errors.Add(new UploadParseError
            {
                RowNumber = 0,
                FieldName = "worksheet",
                ErrorMessage = "Worksheet is empty."
            });
            return Task.FromResult(result);
        }

        var headerMap = new Dictionary<int, string>();
        foreach (var cell in usedRange.Row(1).Cells())
        {
            var value = cell.GetString().Trim();
            if (!string.IsNullOrWhiteSpace(value))
            {
                headerMap[cell.Address.ColumnNumber] = value;
            }
        }

        ValidateHeaders(headerMap.Values.ToArray(), result.Errors);
        if (result.Errors.Count > 0)
        {
            return Task.FromResult(result);
        }

        foreach (var row in usedRange.RowsUsed().Skip(1))
        {
            var rowData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var cell in row.Cells())
            {
                if (headerMap.TryGetValue(cell.Address.ColumnNumber, out var header))
                {
                    rowData[header] = cell.GetString().Trim();
                }
            }

            var mapped = UploadRecordMapper.Map(rowData, row.RowNumber(), result.Errors);
            if (mapped is not null)
            {
                result.Records.Add(mapped);
            }
        }

        return Task.FromResult(result);
    }

    private static void ValidateHeaders(IReadOnlyCollection<string> headers, ICollection<UploadParseError> errors)
    {
        foreach (var required in UploadHeaders.Required)
        {
            if (!headers.Contains(required, StringComparer.OrdinalIgnoreCase))
            {
                errors.Add(new UploadParseError
                {
                    RowNumber = 1,
                    FieldName = required,
                    ErrorMessage = "Required header is missing."
                });
            }
        }
    }
}
