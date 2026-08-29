using System.Diagnostics;

namespace TIMS.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    { _next = next; _logger = logger; }

    public async Task InvokeAsync(HttpContext ctx)
    {
        var sw = Stopwatch.StartNew();
        await _next(ctx);
        sw.Stop();
        _logger.LogInformation("{Method} {Path} → {Status} ({Ms}ms)",
            ctx.Request.Method, ctx.Request.Path,
            ctx.Response.StatusCode, sw.ElapsedMilliseconds);
    }
}
