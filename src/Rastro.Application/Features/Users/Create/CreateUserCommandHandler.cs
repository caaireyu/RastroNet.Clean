using Cortex.Mediator.Commands;
using Rastro.Application.Interfaces;
using Rastro.Application.Features.Users.GetUserById;
using Rastro.Domain.Users;

namespace Rastro.Application.Features.Users.Create;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, UserResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }
    public async Task<UserResponse> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var hashedPassword = _passwordHasher.HashPassword(command.Password);
        var user = User.Create(
            Guid.NewGuid(),
            command.Email,
            command.Name,
            command.LastName,
            command.Email,
            hashedPassword
            );
        _unitOfWork.Users.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new UserResponse(user.Id, user.Email, user.Name, user.LastName ?? "");
    }
}