using Microsoft.AspNetCore.Mvc;
using Cortex.Mediator;
using Rastro.Api.Common;
using Rastro.Application.Features.Users.Create;
using Rastro.Application.Features.Users.GetAll;
using Rastro.Application.Features.Users.GetUserById;

namespace Rastro.Api.Controllers;

[Route("api/users")]
public class UsersController : ApiBaseController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command,
        CancellationToken cancellationToken=default)
    {
        var createdUser = await _mediator.SendCommandAsync<CreateUserCommand, UserResponse>(command, cancellationToken);
        return CustomCreated(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetUserByIdQuery(id);
        var userDetails = await _mediator.SendQueryAsync<GetUserByIdQuery, UserResponse?>(query, cancellationToken);
        return userDetails is not null ? CustomOk(userDetails) : CustomNotFound("User.NotFound","El usuario no existe");
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<UserResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken = default)
    {
        var query = new GetAllUserQuery();
        var users = await _mediator.SendQueryAsync<GetAllUserQuery, List<UserResponse>>(query, cancellationToken);
        return CustomOk(users);
    }
}