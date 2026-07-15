namespace EnterpriseSalesPredictor.Domain.Entities;

public sealed class RolePermission : Entity
{
    public RolePermission(Guid id, Guid roleId, Guid permissionId)
        : base(id)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    public Guid RoleId { get; private set; }

    public Guid PermissionId { get; private set; }

    public Role? Role { get; private set; }

    public Permission? Permission { get; private set; }
}
