using Cortex.Mediator.Commands;
using Rastro.Application.Interfaces;
using Rastro.Domain.Users;

namespace Rastro.Application.Features.Users.Create;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
    public async Task<Guid> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = User.Create(
            Guid.NewGuid(),
            command.Email,
            command.Name,
            command.LastName,
            command.Email,
            command.Password
            );
        _unitOfWork.Users.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return user.Id;
    }
}