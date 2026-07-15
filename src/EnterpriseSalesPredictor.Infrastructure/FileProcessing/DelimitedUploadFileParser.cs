using EnterpriseSalesPredictor.Application.Interfaces.Uploads;

namespace EnterpriseSalesPredictor.Infrastructure.FileProcessing;

public sealed class DelimitedUploadFileParser : IUploadFileParser
{
    private const char Separator = ';';

    public string ParserKey => "delimited";

    public bool CanHandle(string fileName)
    {
        return fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) ||
               fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<UploadParseResult> ParseAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var result = new UploadParseResult();

        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            result.Errors.Add(new UploadParseError
            {
                RowNumber = 0,
                FieldName = "file",
                ErrorMessage = "File is empty."
            });
            return result;
        }

        var lines = content
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .ToArray();

        if (lines.Length < 2)
        {
            result.Errors.Add(new UploadParseError
            {
                RowNumber = 0,
                FieldName = "file",
                ErrorMessage = "File does not contain data rows."
            });
            return result;
        }

        var headers = lines[0].Split(Separator).Select(value => value.Trim()).ToArray();
        ValidateHeaders(headers, result.Errors);
        if (result.Errors.Count > 0)
        {
            return result;
        }

        for (var index = 1; index < lines.Length; index++)
        {
            var values = lines[index].Split(Separator);
            var rowNumber = index + 1;
            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (var col = 0; col < headers.Length; col++)
            {
                var value = col < values.Length ? values[col].Trim() : string.Empty;
                row[headers[col]] = value;
            }

            var mapped = UploadRecordMapper.Map(row, rowNumber, result.Errors);
            if (mapped is not null)
            {
                result.Records.Add(mapped);
            }
        }

        return result;
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
