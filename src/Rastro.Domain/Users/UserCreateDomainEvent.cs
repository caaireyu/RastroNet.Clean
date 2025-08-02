using Rastro.Domain.Primitives;

namespace Rastro.Domain.Users;

public sealed class UserCreateDomainEvent : DomainEvent
{
    public Guid UserId { get; private set; }
    public string Email { get; private set; }
    public string Username { get; private set; }

    public UserCreateDomainEvent(Guid userId, string email, string username)
    {
        UserId = userId;
        Email = email;
        Username = username;
    }
    
}