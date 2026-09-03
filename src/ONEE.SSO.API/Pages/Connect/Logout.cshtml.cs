using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ONEE.SSO.Application.Repositories;

namespace ONEE.SSO.API.Pages.Connect;

/// <summary>
/// Page de déconnexion OIDC complète (Single Sign-Out)
/// Endpoint: /connect/logout
/// </summary>
public class LogoutModel : PageModel
{
    private readonly IUserSessionRepository _sessionRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;

    public LogoutModel(
        IUserSessionRepository sessionRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository)
    {
        _sessionRepository = sessionRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
    }

    [BindProperty(SupportsGet = true)]
    public string? post_logout_redirect_uri { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? id_token_hint { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? state { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Console.WriteLine("[SLO] === Logout endpoint called ===");
        
        // Récupérer l'utilisateur de la session SSO
        var userEmail = HttpContext.Session.GetString("UserEmail");
        var accessToken = HttpContext.Session.GetString("AccessToken");

        Console.WriteLine($"[SLO] UserEmail from session: {userEmail ?? "NULL"}");
        Console.WriteLine($"[SLO] AccessToken from session: {(string.IsNullOrEmpty(accessToken) ? "NULL" : "EXISTS")}");

        // Si pas d'email dans la session, essayer de l'extraire du id_token_hint
        if (string.IsNullOrEmpty(userEmail) && !string.IsNullOrEmpty(id_token_hint))
        {
            try
            {
                // Décoder le JWT (sans validation car on veut juste l'email)
                var parts = id_token_hint.Split('.');
                if (parts.Length == 3)
                {
                    var payload = parts[1];
                    // Ajouter padding si nécessaire
                    payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
                    var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
                    var claims = System.Text.Json.JsonDocument.Parse(json);
                    
                    if (claims.RootElement.TryGetProperty("email", out var emailClaim))
                    {
                        userEmail = emailClaim.GetString();
                        Console.WriteLine($"[SLO] Extracted email from id_token_hint: {userEmail}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SLO] Error extracting email from id_token_hint: {ex.Message}");
            }
        }

        if (!string.IsNullOrEmpty(userEmail))
        {
            Console.WriteLine($"[SLO] Logging out user: {userEmail}");

            try
            {
                // 1. Récupérer l'utilisateur
                var user = await _userRepository.GetByEmailAsync(userEmail);
                
                if (user != null)
                {
                    Console.WriteLine($"[SLO] User found in database: {user.Id}");
                    
                    // 2. Invalider toutes les sessions actives de l'utilisateur
                    var sessions = await _sessionRepository.GetActiveSessionsAsync(user.Id);
                    foreach (var session in sessions)
                    {
                        session.IsActive = false;
                        session.LogoutAt = DateTime.UtcNow;
                        _sessionRepository.Update(session);
                    }
                    await _sessionRepository.SaveChangesAsync();
                    Console.WriteLine($"[SLO] Invalidated {sessions.Count()} active sessions");

                    // 3. Révoquer tous les refresh tokens actifs
                    var refreshTokens = await _refreshTokenRepository.GetActiveTokensByUserIdAsync(user.Id);
                    foreach (var token in refreshTokens)
                    {
                        token.RevokedAt = DateTime.UtcNow;
                        token.RevokedByIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                        _refreshTokenRepository.Update(token);
                    }
                    await _refreshTokenRepository.SaveChangesAsync();
                    Console.WriteLine($"[SLO] Revoked {refreshTokens.Count()} refresh tokens");
                }
                else
                {
                    Console.WriteLine($"[SLO] User not found in database: {userEmail}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SLO] Error during logout cleanup: {ex.Message}");
                // Continuer quand même avec la déconnexion
            }
        }
        else
        {
            Console.WriteLine("[SLO] No user email found, skipping session/token cleanup");
        }

        // 4. Supprimer la session SSO côté serveur
        HttpContext.Session.Clear();
        
        // 5. Supprimer les cookies SSO
        Response.Cookies.Delete(".AspNetCore.Session");
        Response.Cookies.Delete("AspNetCore.Cookies");
        
        Console.WriteLine("[SLO] SSO session cleared and cookies deleted");

        // 6. Rediriger vers l'application cliente ou la page de login
        if (!string.IsNullOrEmpty(post_logout_redirect_uri))
        {
            var redirectUrl = post_logout_redirect_uri;
            if (!string.IsNullOrEmpty(state))
            {
                redirectUrl += (redirectUrl.Contains("?") ? "&" : "?") + $"state={state}";
            }
            
            Console.WriteLine($"[SLO] Redirecting to client: {redirectUrl}");
            return Redirect(redirectUrl);
        }

        // Pas de redirect_uri spécifiée, rediriger vers page de logout SSO
        return RedirectToPage("/Logout");
    }
}
