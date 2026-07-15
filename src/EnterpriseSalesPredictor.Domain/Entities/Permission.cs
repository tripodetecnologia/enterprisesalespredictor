namespace EnterpriseSalesPredictor.Domain.Entities;

public sealed class Permission : Entity
{
    public Permission(Guid id, string code)
        : base(id)
    {
        Code = code;
    }

    public string Code { get; private set; }
}
