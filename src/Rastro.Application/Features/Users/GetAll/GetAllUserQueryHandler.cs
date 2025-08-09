using Cortex.Mediator.Queries;
using Rastro.Application.Features.Users.GetUserById;
using Rastro.Application.Interfaces;
using Rastro.Application.Interfaces.Repositories;

namespace Rastro.Application.Features.Users.GetAll;

public class GetAllUserQueryHandler : IQueryHandler<GetAllUserQuery, List<UserResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllUserQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }


    public async Task<List<UserResponse>> Handle(GetAllUserQuery query, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);

        return users.Select(user => new UserResponse(user.Id, user.Email, user.Name, user.LastName ?? "")).ToList();
    }
}