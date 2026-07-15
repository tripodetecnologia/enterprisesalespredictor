namespace EnterpriseSalesPredictor.Domain.Entities;

public sealed class Role : Entity
{
    private readonly List<RolePermission> _rolePermissions = new();

    public Role(Guid id, string name)
        : base(id)
    {
        Name = name;
    }

    public string Name { get; private set; }

    public ICollection<RolePermission> RolePermissions => _rolePermissions;

    public void AddPermission(RolePermission rolePermission)
    {
        _rolePermissions.Add(rolePermission);
    }

    public void ReplacePermissions(IEnumerable<RolePermission> permissions)
    {
        _rolePermissions.Clear();
        _rolePermissions.AddRange(permissions);
    }
}
