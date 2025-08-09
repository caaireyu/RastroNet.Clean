using System.Net;
using System.Text.Json;
using Rastro.Api.Common;
using Rastro.Domain.Exceptions;

namespace Rastro.Api.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DuplicateUserEmailException ex)
        {
            _logger.LogError(ex,"Intento de registro con email duplicado: {Message}", ex.Message);

            context.Response.StatusCode = (int)HttpStatusCode.Conflict; //409
            context.Response.ContentType = "application/json";

            var error = new ApiError("User.DuplicateEmail", ex.Message);
            var response = new ApiResponse<object> { IsSuccess = false, Errors = [error] };
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Ha ocurrido un error inesperado: {Message}", ex.Message);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var error = new ApiError("ServerError", "Ha ocurrido un error inesperado. Inténtelo más tarde");
            var response = new ApiResponse<object> { IsSuccess = false, Errors = [error] };
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        
    }
}