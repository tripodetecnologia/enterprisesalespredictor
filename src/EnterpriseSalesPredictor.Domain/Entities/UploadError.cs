namespace EnterpriseSalesPredictor.Domain.Entities;

public sealed class UploadError : Entity
{
    public UploadError(Guid id, Guid uploadedFileId, int rowNumber, string fieldName, string errorMessage)
        : base(id)
    {
        UploadedFileId = uploadedFileId;
        RowNumber = rowNumber;
        FieldName = fieldName;
        ErrorMessage = errorMessage;
    }

    public Guid UploadedFileId { get; private set; }

    public int RowNumber { get; private set; }

    public string FieldName { get; private set; }

    public string ErrorMessage { get; private set; }
}
