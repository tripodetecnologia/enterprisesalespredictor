using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ClosedXML.Excel;
using EnterpriseSalesPredictor.Infrastructure.FileProcessing;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using EnterpriseSalesPredictor.Tests.Integration.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseSalesPredictor.Tests.Integration.Api;

[TestFixture]
public sealed class ApiIntegrationTests
{
    private ApiWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        _factory = new ApiWebApplicationFactory();
        _client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task ProtectedEndpoint_ShouldRequireAuthorization()
    {
        var response = await _client.GetAsync("api/sales/range");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Login_ShouldReturnAccessToken()
    {
        var response = await _client.PostAsJsonAsync("api/auth/login", new { username = "devadmin", password = "DevAdmin@123" });
        var payload = await ParseJsonAsync(response);

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(payload.GetProperty("accessToken").GetString(), Is.Not.Empty);
        });
    }

    [Test]
    public async Task SalesEndpoint_ShouldReturnResultsForAuthorizedUser()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync("api/sales/range?PageSize=5");
        var payload = await ParseJsonAsync(response);

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(payload.ValueKind, Is.EqualTo(JsonValueKind.Object));
            Assert.That(payload.GetProperty("items").GetArrayLength(), Is.EqualTo(5));
        });
    }

    [Test]
    public async Task AccessManagement_ShouldPersistCreatedUserAndAllowLogin()
    {
        await AuthenticateAsync();

        var createResponse = await _client.PostAsJsonAsync("api/access/users", new
        {
            Username = "persistent-user",
            Password = "Persistent@123",
            Role = "Supervisor",
            Permissions = new[] { "sales:read", "reports:read" }
        });

        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        using var loginClient = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var loginResponse = await loginClient.PostAsJsonAsync("api/auth/login", new { username = "persistent-user", password = "Persistent@123" });
        var loginPayload = await ParseJsonAsync(loginResponse);

        Assert.That(loginPayload.GetProperty("accessToken").GetString(), Is.Not.Empty);
    }

    [Test]
    public async Task UploadDelimitedEndpoint_ShouldPersistImportedData()
    {
        await AuthenticateAsync();

        using var content = new MultipartFormDataContent();
        var csv = BuildDelimitedUploadContent();
        content.Add(new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(csv)), "file", "sales.csv");

        var response = await _client.PostAsync("api/uploads/delimited", content);
        var payload = await ParseJsonAsync(response);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(payload.GetProperty("validRecords").GetInt32(), Is.GreaterThanOrEqualTo(1));
            Assert.That(dbContext.UploadedFiles.Count(), Is.GreaterThanOrEqualTo(1));
        });
    }

    [Test]
    public async Task UploadExcelEndpoint_ShouldAcceptWorkbook()
    {
        await AuthenticateAsync();

        using var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(BuildExcelUploadContent()), "file", "sales.xlsx");

        var response = await _client.PostAsync("api/uploads/excel", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task ExportReportsEndpoint_ShouldReturnExcelFile()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync("api/exports/reports");
        var bytes = await response.Content.ReadAsByteArrayAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.That(bytes.Length, Is.GreaterThan(0));
        });
    }

    [Test]
    public async Task AuditEndpoint_ShouldReturnRecordedEntries()
    {
        await AuthenticateAsync();
        await _client.GetAsync("api/exports/base-data");

        var response = await _client.GetAsync("api/audit");
        var payload = await ParseJsonAsync(response);

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(payload.ValueKind, Is.EqualTo(JsonValueKind.Array));
            Assert.That(payload.GetArrayLength(), Is.GreaterThan(0));
        });
    }

    private async Task AuthenticateAsync()
    {
        var response = await _client.PostAsJsonAsync("api/auth/login", new { username = "devadmin", password = "DevAdmin@123" });
        var payload = await ParseJsonAsync(response);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", payload.GetProperty("accessToken").GetString());
    }

    private static async Task<JsonElement> ParseJsonAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.Clone();
    }

    private static string BuildDelimitedUploadContent()
    {
        var values = new[]
        {
            "INV-UP-01",
            "Client Test",
            "CT-01",
            "Client Address",
            "Quito",
            "555-321",
            "North",
            "Hardware",
            "Valve",
            "VAL-01",
            "BrandY",
            "12.5",
            "18.5",
            "20",
            "2",
            "37",
            "2026-03-15",
            "Seller Test",
            "SELL-01",
            "Supplier Test",
            "SUP-01",
            "Supplier Address",
            "555-876",
            "Quito",
            "Cash"
        };

        return string.Join(";", UploadHeaders.Required) + Environment.NewLine + string.Join(";", values);
    }

    private static byte[] BuildExcelUploadContent()
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Upload");

        for (var column = 0; column < UploadHeaders.Required.Length; column++)
        {
            sheet.Cell(1, column + 1).Value = UploadHeaders.Required[column];
        }

        var values = new[]
        {
            "INV-UP-02",
            "Client Excel",
            "CT-02",
            "Client Address 2",
            "Quito",
            "555-322",
            "North",
            "Hardware",
            "Sensor",
            "SEN-01",
            "BrandZ",
            "10.5",
            "16.5",
            "30",
            "3",
            "49.5",
            "2026-03-16",
            "Seller Excel",
            "SELL-02",
            "Supplier Excel",
            "SUP-02",
            "Supplier Address 2",
            "555-877",
            "Quito",
            "Card"
        };

        for (var column = 0; column < values.Length; column++)
        {
            sheet.Cell(2, column + 1).Value = values[column];
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
