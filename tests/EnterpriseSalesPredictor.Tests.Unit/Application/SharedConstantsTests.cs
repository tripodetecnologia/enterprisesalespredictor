using EnterpriseSalesPredictor.Application.Constants;

namespace EnterpriseSalesPredictor.Tests.Unit.Application;

public sealed class SharedConstantsTests
{
    [Test]
    public void PermissionPolicies_ShouldBuildPolicyNamesFromPermissionCodes()
    {
        Assert.That(PermissionPolicies.ReplenishmentWrite, Is.EqualTo($"{PermissionPolicies.Prefix}{Permissions.ReplenishmentWrite}"));
        Assert.That(PermissionPolicies.UploadsRead, Is.EqualTo($"{PermissionPolicies.Prefix}{Permissions.UploadsRead}"));
    }

    [Test]
    public void UploadPolicy_ShouldExposeConsistentUploadContract()
    {
        Assert.That(UploadPolicy.MaxFileSizeBytes, Is.EqualTo(UploadPolicy.MaxFileSizeMegabytes * UploadPolicy.BytesPerMegabyte));
        Assert.That(UploadPolicy.AllExtensions, Is.EquivalentTo(new[] { ".xlsx", ".xls", ".csv", ".txt" }));
        Assert.That(UploadPolicy.AcceptAttribute, Is.EqualTo(".xlsx,.xls,.csv,.txt"));
    }

    [Test]
    public void ExportFormats_ShouldExposeExcelContract()
    {
        Assert.That(ExportFormats.ExcelContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
        Assert.That(ExportFormats.ExcelExtension, Is.EqualTo(".xlsx"));
    }
}
