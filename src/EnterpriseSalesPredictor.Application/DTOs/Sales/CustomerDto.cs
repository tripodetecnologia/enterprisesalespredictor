namespace EnterpriseSalesPredictor.Application.DTOs.Sales;

public sealed class CustomerDto
{
    public Guid Id { get; set; }

    public string Identification { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Zone { get; set; } = string.Empty;
}
