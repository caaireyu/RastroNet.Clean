using Rastro.Application.Features.Users.GetUserById;
using Rastro.Application.Interfaces;

namespace Rastro.Application.Features.Users.Create;

public record CreateUserCommand(string Email, string Name, string LastName, string Password) : ITransactionalCommand<UserResponse>;