using Rastro.Domain.Primitives;

namespace Rastro.Domain.Users;

public sealed class User: AggregateRoot
{
    public Guid Id { get; private set; }
    public string Username { get; private set; } = default!;

    public string Name { get; private set; } = default!;
    public string? LastName { get; private set; }
    public string Email { get; private set; } = default!;
    public string Password { get; set; } = default!;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    
    public User(){}

    public static User Create(Guid id, string username, string name, string? lastName, string email, string password)
    {
        var user = new User
        {
            Id = id,
            Username = username,
            Name = name,
            LastName = lastName,
            Email = email,
            Password = password,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        // Domain Event
        user.RaiseDomainEvent(new UserCreateDomainEvent(user.Id, user.Email, user.Username));
        return user;
    }
    
}