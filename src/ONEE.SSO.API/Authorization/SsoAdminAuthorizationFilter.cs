using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ONEE.SSO.Application.Repositories;

namespace ONEE.SSO.API.Authorization;

/// <summary>
/// Filtre d'autorisation pour les pages admin du SSO.
/// Vérifie que l'utilisateur est connecté ET qu'il a le flag IsSsoAdmin = true.
/// </summary>
public class SsoAdminAuthorizationFilter : IAsyncAuthorizationFilter
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SsoAdminAuthorizationFilter> _logger;

    public SsoAdminAuthorizationFilter(
        IUserRepository userRepository,
        ILogger<SsoAdminAuthorizationFilter> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var httpContext = context.HttpContext;

        // 1. Vérifier si l'utilisateur est connecté (session)
        var userEmail = httpContext.Session.GetString("UserEmail");
        var userId = httpContext.Session.GetString("UserId");

        if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("🚫 Accès refusé: Utilisateur non connecté");
            context.Result = new RedirectToPageResult("/Login", new { returnUrl = httpContext.Request.Path });
            return;
        }

        // 2. Vérifier que l'utilisateur existe et a le flag IsSsoAdmin
        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));

        if (user == null)
        {
            _logger.LogWarning($"🚫 Accès refusé: Utilisateur {userId} introuvable");
            httpContext.Session.Clear();
            context.Result = new RedirectToPageResult("/Login");
            return;
        }

        if (!user.IsSsoAdmin)
        {
            _logger.LogWarning($"🚫 Accès refusé: {user.Email} n'est pas un SSO Admin");
            context.Result = new ForbidResult();
            return;
        }

        _logger.LogInformation($"✅ Accès autorisé: {user.Email} (SSO Admin)");
    }
}

/// <summary>
/// Attribut à placer sur les PageModel des pages admin.
/// Exemple: [SsoAdminRequired]
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class SsoAdminRequiredAttribute : Attribute, IFilterFactory
{
    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
        var logger = serviceProvider.GetRequiredService<ILogger<SsoAdminAuthorizationFilter>>();
        return new SsoAdminAuthorizationFilter(userRepository, logger);
    }
}
