namespace EnterpriseSalesPredictor.Domain.Entities;

public sealed class UserRole : Entity
{
    public UserRole(Guid id, Guid userId, Guid roleId)
        : base(id)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public Guid UserId { get; private set; }

    public Guid RoleId { get; private set; }

    public User? User { get; private set; }

    public Role? Role { get; private set; }
}
