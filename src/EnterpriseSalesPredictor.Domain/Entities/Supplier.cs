namespace EnterpriseSalesPredictor.Domain.Entities;

public sealed class Supplier : Entity
{
    public Supplier(
        Guid id,
        string identification,
        string name,
        string city,
        string? address,
        string? phone)
        : base(id)
    {
        Identification = identification;
        Name = name;
        City = city;
        Address = address;
        Phone = phone;
    }

    public string Identification { get; private set; }

    public string Name { get; private set; }

    public string City { get; private set; }

    public string? Address { get; private set; }

    public string? Phone { get; private set; }
}
