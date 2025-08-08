using Cortex.Mediator.Queries;

namespace Rastro.Application.Features.Users.GetUserById;

public record GetUserByIdQuery(Guid Id) : IQuery<UserResponse?>;