namespace EnterpriseSalesPredictor.Web.ViewModels.Audit;

public sealed class AuditFilterViewModel
{
    public string? Module { get; set; }

    public string? Actor { get; set; }

    public DateTime? FromUtc { get; set; }

    public DateTime? ToUtc { get; set; }
}
