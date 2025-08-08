using Cortex.Mediator.Queries;
using Rastro.Application.Interfaces;

namespace Rastro.Application.Features.Users.GetUserById;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserResponse?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
    public async Task<UserResponse?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(query.Id, cancellationToken);
        return user is null ? null : new UserResponse(user.Id, user.Email, user.Name, user.LastName ?? "");
    }
}