namespace EnterpriseSalesPredictor.Domain.Entities;

public enum UploadProcessStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    CompletedWithErrors = 3,
    Rejected = 4
}
