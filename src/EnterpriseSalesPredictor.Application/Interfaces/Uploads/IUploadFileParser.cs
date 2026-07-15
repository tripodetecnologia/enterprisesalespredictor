namespace EnterpriseSalesPredictor.Application.Interfaces.Uploads;

public interface IUploadFileParser
{
    string ParserKey { get; }

    bool CanHandle(string fileName);

    Task<UploadParseResult> ParseAsync(Stream stream, CancellationToken cancellationToken = default);
}
