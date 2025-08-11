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
        
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        (HttpStatusCode statusCode, ApiError error) = exception switch
        {
            DuplicateUserEmailException ex => (HttpStatusCode.Conflict,
                new ApiError("User.DuplicateEmail", ex.Message)),
            _ => (HttpStatusCode.InternalServerError,
                new ApiError("ServerError", "Ha ocurrido un error inesperado. Inténtelo más tarde"))

        };
        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Ha ocurrido un error inesperado {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning(exception, "Excepción de negocio controlada {Message}", exception.Message);
        }
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        var response = new ApiResponse<object> { IsSuccess = false, Errors = [error] };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}