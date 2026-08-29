using System.Net;
using System.Text.Json;
using FluentValidation;

namespace GestionPersonnel.API.Exceptions;

public class GlobalExceptionHandler : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Erreur de validation: {Message}", ex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            var errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage });
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status = 400,
                message = "Erreur de validation.",
                errors
            }));
        }
        catch (AppException ex)
        {
            _logger.LogWarning("AppException [{StatusCode}]: {Message}", ex.StatusCode, ex.Message);
            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status = ex.StatusCode,
                message = ex.Message
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur interne non gérée.");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status = 500,
                message = "Une erreur interne s'est produite."
            }));
        }
    }
}
