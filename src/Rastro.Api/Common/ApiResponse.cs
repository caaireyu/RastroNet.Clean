namespace Rastro.Api.Common;

public record ApiError(string Code, string Message);

public class ApiResponse<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public List<ApiError> Errors { get; init; } = [];

}