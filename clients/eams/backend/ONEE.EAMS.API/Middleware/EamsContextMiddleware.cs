namespace ONEE.EAMS.API.Middleware;

public class EamsContextMiddleware
{
    private readonly RequestDelegate _next;

    public EamsContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Récupérer les custom claims depuis le token JWT
        var eamsUserId = context.User.FindFirst("eams_user_id")?.Value;
        var serviceId = context.User.FindFirst("serviceId")?.Value;

        // Ajouter dans HttpContext.Items
        context.Items["EamsUserId"] = eamsUserId;
        context.Items["ServiceId"] = serviceId;

        await _next(context);
    }
}

public static class EamsContextMiddlewareExtensions
{
    public static IApplicationBuilder UseEamsContext(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<EamsContextMiddleware>();
    }
}
