using Cortex.Mediator.Queries;
using Rastro.Application.Features.Users.GetUserById;

namespace Rastro.Application.Features.Users.GetAll;

public record GetAllUserQuery : IQuery<List<UserResponse>>;
