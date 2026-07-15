namespace EnterpriseSalesPredictor.Domain.Entities;

public sealed class Customer : Entity
{
    public Customer(
        Guid id,
        string identification,
        string name,
        string city,
        string zone,
        string? address,
        string? phone)
        : base(id)
    {
        Identification = identification;
        Name = name;
        City = city;
        Zone = zone;
        Address = address;
        Phone = phone;
    }

    public string Identification { get; private set; }

    public string Name { get; private set; }

    public string City { get; private set; }

    public string Zone { get; private set; }

    public string? Address { get; private set; }

    public string? Phone { get; private set; }
}
