namespace EnterpriseSalesPredictor.Domain.Entities;

public sealed class User : Entity
{
    private readonly List<UserRole> _userRoles = new();

    public User(Guid id, string username, string passwordHash, bool isActive, DateTime createdAtUtc)
        : base(id)
    {
        Username = username;
        PasswordHash = passwordHash;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
    }

    public string Username { get; private set; }

    public string PasswordHash { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public ICollection<UserRole> UserRoles => _userRoles;

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void AddRole(UserRole userRole)
    {
        _userRoles.Add(userRole);
    }
}
