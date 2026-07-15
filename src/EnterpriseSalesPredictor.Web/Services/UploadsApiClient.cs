using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using EnterpriseSalesPredictor.Web.ViewModels.Uploads;

namespace EnterpriseSalesPredictor.Web.Services;

public sealed class UploadsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UploadsApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IReadOnlyCollection<UploadSessionViewModel>> GetUploadsAsync(CancellationToken cancellationToken = default)
    {
        AttachBearerToken();

        var payload = await _httpClient.GetFromJsonAsync<UploadSessionViewModel[]>("api/uploads", cancellationToken);
        return payload ?? Array.Empty<UploadSessionViewModel>();
    }

    public async Task<IReadOnlyCollection<UploadErrorViewModel>> GetUploadErrorsAsync(Guid uploadId, CancellationToken cancellationToken = default)
    {
        AttachBearerToken();

        var payload = await _httpClient.GetFromJsonAsync<UploadErrorViewModel[]>($"api/uploads/{uploadId}/errors", cancellationToken);
        return payload ?? Array.Empty<UploadErrorViewModel>();
    }

    public async Task<UploadProcessingResponseViewModel> UploadExcelAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        return await UploadFileAsync("api/uploads/excel", fileStream, fileName, cancellationToken);
    }

    public async Task<UploadProcessingResponseViewModel> UploadDelimitedAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        return await UploadFileAsync("api/uploads/delimited", fileStream, fileName, cancellationToken);
    }

    private async Task<UploadProcessingResponseViewModel> UploadFileAsync(string endpoint, Stream fileStream, string fileName, CancellationToken cancellationToken)
    {
        AttachBearerToken();

        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(streamContent, "file", fileName);

        var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"API rejected upload: {error}");
        }

        var payload = await response.Content.ReadFromJsonAsync<UploadProcessingResponseViewModel>(cancellationToken: cancellationToken);
        if (payload is null)
        {
            throw new InvalidOperationException("API returned an invalid upload response.");
        }

        return payload;
    }

    private void AttachBearerToken()
    {
        var accessToken = _httpContextAccessor.HttpContext?.User.FindFirstValue("access_token");
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}
