using Microsoft.AspNetCore.Mvc;
using Cortex.Mediator;
using Rastro.Application.Features.Users.Create;
using Rastro.Application.Features.Users.GetUserById;

namespace Rastro.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command,
        CancellationToken cancellationToken=default)
    {
        var createdUser = await _mediator.SendCommandAsync<CreateUserCommand, UserResponse>(command, cancellationToken);
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetUserByIdQuery(id);
        var userDetails = await _mediator.SendQueryAsync<GetUserByIdQuery, UserResponse?>(query, cancellationToken);
        return userDetails is not null ? Ok(userDetails) : NotFound();
    }
}