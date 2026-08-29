using System.Text.Json;
using ONEE.EAMS.Application.Common;

namespace ONEE.EAMS.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception non gérée: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, details) = ex switch
        {
            NotFoundException e => (404, e.Message, (IEnumerable<string>?)[]),
            ForbiddenException e => (403, e.Message, (IEnumerable<string>?)[]),
            UnauthorizedException e => (401, e.Message, (IEnumerable<string>?)[]),
            ConflictException e => (409, e.Message, (IEnumerable<string>?)[]),
            Application.Common.ValidationException e => (422, "Validation échouée.", (IEnumerable<string>?)e.Errors),
            _ => (500, "Une erreur interne est survenue.", (IEnumerable<string>?)[])
        };

        context.Response.StatusCode = statusCode;
        var response = ApiResponse<object>.Fail(message, statusCode, details);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(json);
    }
}
