namespace TIMS.API.Middleware;

public class TimsContextMiddleware
{
    private readonly RequestDelegate _next;

    public TimsContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Récupérer les custom claims depuis le token JWT
        var timsUserId = context.User.FindFirst("tims_user_id")?.Value;
        var timsServiceId = context.User.FindFirst("tims_service_id")?.Value;
        var timsTeamId = context.User.FindFirst("tims_team_id")?.Value;

        // Ajouter dans HttpContext.Items
        context.Items["TimsUserId"] = timsUserId;
        context.Items["TimsServiceId"] = timsServiceId;
        context.Items["TimsTeamId"] = timsTeamId;

        await _next(context);
    }
}

public static class TimsContextMiddlewareExtensions
{
    public static IApplicationBuilder UseTimsContext(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TimsContextMiddleware>();
    }
}
