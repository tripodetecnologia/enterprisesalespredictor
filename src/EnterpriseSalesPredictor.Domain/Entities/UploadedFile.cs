namespace EnterpriseSalesPredictor.Domain.Entities;

public sealed class UploadedFile : Entity
{
    public UploadedFile(
        Guid id,
        string fileName,
        string fileType,
        DateTime uploadedAtUtc,
        string uploadedBy,
        UploadProcessStatus status)
        : base(id)
    {
        FileName = fileName;
        FileType = fileType;
        UploadedAtUtc = uploadedAtUtc;
        UploadedBy = uploadedBy;
        Status = status;
    }

    public string FileName { get; private set; }

    public string FileType { get; private set; }

    public DateTime UploadedAtUtc { get; private set; }

    public string UploadedBy { get; private set; }

    public UploadProcessStatus Status { get; private set; }

    public int TotalRecords { get; private set; }

    public int ValidRecords { get; private set; }

    public int InvalidRecords { get; private set; }

    public void Complete(int totalRecords, int validRecords, int invalidRecords, UploadProcessStatus status)
    {
        TotalRecords = totalRecords;
        ValidRecords = validRecords;
        InvalidRecords = invalidRecords;
        Status = status;
    }
}
