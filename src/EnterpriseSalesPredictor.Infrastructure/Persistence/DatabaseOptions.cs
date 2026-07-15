namespace EnterpriseSalesPredictor.Infrastructure.Persistence;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public const string DefaultConnectionName = "DefaultConnection";

    public string Provider { get; set; } = "MySql";

    public string ConnectionString { get; set; } = string.Empty;

    public int CommandTimeoutSeconds { get; set; } = 30;
}
