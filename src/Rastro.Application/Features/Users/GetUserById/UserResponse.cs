namespace Rastro.Application.Features.Users.GetUserById;

public record UserResponse(Guid Id, string Email, string Name, string LastName);