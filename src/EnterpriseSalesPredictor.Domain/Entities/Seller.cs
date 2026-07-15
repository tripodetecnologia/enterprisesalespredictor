namespace EnterpriseSalesPredictor.Domain.Entities;

public sealed class Seller : Entity
{
    public Seller(Guid id, string identification, string name)
        : base(id)
    {
        Identification = identification;
        Name = name;
    }

    public string Identification { get; private set; }

    public string Name { get; private set; }
}
