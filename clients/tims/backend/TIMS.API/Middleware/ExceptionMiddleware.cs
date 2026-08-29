using System.Text.Json;
using TIMS.API.Common;

namespace TIMS.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    { _next = next; _logger = logger; }

    public async Task InvokeAsync(HttpContext ctx)
    {
        try { await _next(ctx); }
        catch (Exception ex) { await HandleAsync(ctx, ex); }
    }

    private async Task HandleAsync(HttpContext ctx, Exception ex)
    {
        int status; string code; List<string>? errors = null;

        switch (ex)
        {
            case Common.ValidationException ve:
                status = 422; code = ve.ErrorCode; errors = ve.ValidationErrors; break;
            case AppException ae:
                status = ae.StatusCode; code = ae.ErrorCode; break;
            default:
                _logger.LogError(ex, "Erreur non gérée");
                status = 500; code = "INTERNAL_ERROR"; break;
        }

        if (status is 401 or 403)
            _logger.LogWarning("Accès refusé [{Code}] {Path}", code, ctx.Request.Path);

        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = "application/json";

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = ex.Message,
            Errors = errors
        };

        await ctx.Response.WriteAsync(JsonSerializer.Serialize(response,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
