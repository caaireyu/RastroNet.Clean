using Microsoft.AspNetCore.Mvc;
using Rastro.Api.Common;

namespace Rastro.Api.Controllers;

[ApiController]
public class ApiBaseController : ControllerBase
{
    protected IActionResult CustomOk<T>(T data)
    {
        return Ok(new ApiResponse<T> { IsSuccess = true, Data = data });
    }

    protected IActionResult CustomCreated<T>(string actionName, object? routeValues, T data)
    {
        return CreatedAtAction(actionName, routeValues, new ApiResponse<T> { IsSuccess = true, Data = data });
    }

    protected IActionResult CustomNotFound(string code, string message)
    {
        var error = new ApiError(code, message);
        return NotFound(new ApiResponse<object> { IsSuccess = false, Errors = [error] });
    }

    protected IActionResult CustomBadRequest(List<ApiError> errors)
    {
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Errors = errors });
    }

    protected IActionResult CustomUnauthorized(string code, string message)
    {
        var error = new ApiError(code, message);
        return Unauthorized(new ApiResponse<object> { IsSuccess = false, Errors = [error] });
    }
    
}