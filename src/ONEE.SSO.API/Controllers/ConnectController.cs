using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ONEE.SSO.API.Services;
using ONEE.SSO.Application.Repositories;

namespace ONEE.SSO.API.Controllers;

/// <summary>
/// Contrôleur pour les endpoints OIDC/OAuth2 (token exchange)
/// </summary>
[ApiController]
[Route("connect")]
public class ConnectController : ControllerBase
{
    private readonly AuthorizationCodeStore _codeStore;
    private readonly ILogger<ConnectController> _logger;
    private readonly IUserRepository _userRepository;

    public ConnectController(
        AuthorizationCodeStore codeStore,
        ILogger<ConnectController> logger,
        IUserRepository userRepository)
    {
        _codeStore = codeStore;
        _logger = logger;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Logout endpoint OIDC
    /// GET /connect/logout
    /// </summary>
    [HttpGet("logout")]
    public IActionResult Logout([FromQuery] string? id_token_hint, [FromQuery] string? post_logout_redirect_uri)
    {
        _logger.LogInformation("=== LOGOUT ENDPOINT CALLED ===");
        _logger.LogInformation("id_token_hint: {IdTokenHint}", id_token_hint);
        _logger.LogInformation("post_logout_redirect_uri: {PostLogoutRedirectUri}", post_logout_redirect_uri);

        // Nettoyer la session
        HttpContext.Session.Clear();

        // Rediriger vers l'application cliente ou vers la page de logout SSO
        if (!string.IsNullOrEmpty(post_logout_redirect_uri))
        {
            _logger.LogInformation("Redirecting to post_logout_redirect_uri: {Uri}", post_logout_redirect_uri);
            return Redirect(post_logout_redirect_uri);
        }

        // Par défaut, rediriger vers la page de logout du SSO
        return Redirect("/Logout");
    }

    /// <summary>
    /// Token endpoint pour échanger un authorization code contre un access token
    /// POST /connect/token
    /// </summary>
    [HttpPost("token")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> Token([FromForm] TokenRequest request)
    {
        // Log ALL form data received
        _logger.LogInformation("=== TOKEN ENDPOINT CALLED ===");
        _logger.LogInformation("Raw query string: {QueryString}", Request.QueryString.Value);
        
        if (Request.HasFormContentType)
        {
            _logger.LogInformation("Form data received:");
            foreach (var kvp in Request.Form)
            {
                _logger.LogInformation("  {Key} = {Value} (length={Length})", kvp.Key, kvp.Value, kvp.Value.ToString().Length);
            }
        }
        
        _logger.LogInformation("Parsed request object:");
        _logger.LogInformation("  grant_type = {GrantType}", request.grant_type);
        _logger.LogInformation("  code = {Code} (length={CodeLength})", request.code, request.code?.Length ?? 0);
        _logger.LogInformation("  client_id = {ClientId}", request.client_id);
        _logger.LogInformation("  redirect_uri = {RedirectUri}", request.redirect_uri);
        _logger.LogInformation("  code_verifier = {CodeVerifier}", request.code_verifier);

        // Valider grant_type
        if (request.grant_type != "authorization_code")
        {
            _logger.LogWarning("Unsupported grant_type: {GrantType}", request.grant_type);
            return BadRequest(new TokenErrorResponse
            {
                error = "unsupported_grant_type",
                error_description = "Le grant_type doit être 'authorization_code'"
            });
        }

        // Valider les paramètres obligatoires
        if (string.IsNullOrEmpty(request.code))
        {
            return BadRequest(new TokenErrorResponse
            {
                error = "invalid_request",
                error_description = "Le paramètre 'code' est obligatoire"
            });
        }

        if (string.IsNullOrEmpty(request.client_id))
        {
            return BadRequest(new TokenErrorResponse
            {
                error = "invalid_request",
                error_description = "Le paramètre 'client_id' est obligatoire"
            });
        }

        if (string.IsNullOrEmpty(request.redirect_uri))
        {
            return BadRequest(new TokenErrorResponse
            {
                error = "invalid_request",
                error_description = "Le paramètre 'redirect_uri' est obligatoire"
            });
        }

        // Récupérer le token depuis le store
        var codeData = _codeStore.ConsumeCode(request.code);

        if (codeData == null)
        {
            _logger.LogWarning("Authorization code not found or expired: {Code}", request.code);
            return BadRequest(new TokenErrorResponse
            {
                error = "invalid_grant",
                error_description = "Le code d'autorisation est invalide ou a expiré"
            });
        }

        // Vérifier que le client_id correspond
        if (codeData.ClientId != request.client_id)
        {
            _logger.LogWarning("Client ID mismatch: stored={StoredClientId}, received={ReceivedClientId}", 
                codeData.ClientId, request.client_id);
            return BadRequest(new TokenErrorResponse
            {
                error = "invalid_grant",
                error_description = "Le code d'autorisation n'appartient pas à ce client"
            });
        }

        // Extraire l'email du code data (stocké lors de l'autorisation)
        var userEmail = codeData.UserEmail;
        if (string.IsNullOrEmpty(userEmail))
        {
            _logger.LogError("UserEmail not found in code data for code: {Code}", request.code);
            return BadRequest(new TokenErrorResponse
            {
                error = "server_error",
                error_description = "Données utilisateur introuvables"
            });
        }

        // Récupérer l'utilisateur depuis la base
        var user = await _userRepository.GetByEmailAsync(userEmail);
        if (user == null || !user.IsActive)
        {
            _logger.LogError("User not found or inactive: {Email}", userEmail);
            return BadRequest(new TokenErrorResponse
            {
                error = "invalid_grant",
                error_description = "Utilisateur introuvable ou inactif"
            });
        }

        // Générer un nouveau JWT valide avec IJwtService
        var jwtService = HttpContext.RequestServices.GetRequiredService<ONEE.SSO.Application.Interfaces.IJwtService>();
        var userRoleRepository = HttpContext.RequestServices.GetRequiredService<ONEE.SSO.Application.Repositories.IUserRoleRepository>();
        var rolePermissionRepository = HttpContext.RequestServices.GetRequiredService<ONEE.SSO.Application.Repositories.IRolePermissionRepository>();
        
        // Récupérer les rôles de l'utilisateur
        var userRoles = await userRoleRepository.GetByUserIdAsync(user.Id);
        var roles = userRoles.Select(ur => ur.Role.Name).Distinct().ToList();

        // Récupérer les permissions
        var permissions = new List<string>();
        foreach (var userRole in userRoles)
        {
            var rolePermissions = await rolePermissionRepository.GetByRoleIdAsync(userRole.RoleId);
            permissions.AddRange(rolePermissions.Select(rp => rp.Permission.Code));
        }
        permissions = permissions.Distinct().ToList();
        
        // Générer le nouveau access_token avec rôles et permissions
        var newAccessToken = jwtService.GenerateAccessToken(user.Id, user.Email, roles, permissions);
        
        // Générer l'id_token OIDC (contient les infos d'identité de l'utilisateur)
        var fullName = $"{user.FirstName} {user.LastName}".Trim();
        var idToken = jwtService.GenerateIdToken(user.Id, user.Email, fullName, request.client_id!);

        _logger.LogInformation("✅ Generated access_token and id_token for user: {Email}, client: {ClientId}", user.Email, request.client_id);

        // Retourner BOTH access_token AND id_token (OIDC requirement)
        var response = new TokenResponse
        {
            access_token = newAccessToken,
            id_token = idToken, // ✅ ID Token ajouté (requis par OIDC spec)
            token_type = "Bearer",
            expires_in = 3600, // 1 heure
            scope = "openid profile email roles permissions"
        };

        _logger.LogInformation("Token exchange successful for client_id={ClientId}", request.client_id);

        return Ok(response);
    }

    /// <summary>
    /// Modèle pour la requête de token
    /// </summary>
    public class TokenRequest
    {
        public string? grant_type { get; set; }
        public string? code { get; set; }
        public string? redirect_uri { get; set; }
        public string? client_id { get; set; }
        public string? client_secret { get; set; }
        public string? code_verifier { get; set; } // Pour PKCE
    }

    /// <summary>
    /// Modèle pour la réponse de token (succès)
    /// </summary>
    public class TokenResponse
    {
        public string access_token { get; set; } = string.Empty;
        public string token_type { get; set; } = "Bearer";
        public int expires_in { get; set; }
        public string? refresh_token { get; set; }
        public string? scope { get; set; }
        public string? id_token { get; set; } // Pour OpenID Connect
    }

    /// <summary>
    /// Modèle pour la réponse d'erreur
    /// </summary>
    public class TokenErrorResponse
    {
        public string error { get; set; } = string.Empty;
        public string? error_description { get; set; }
        public string? error_uri { get; set; }
    }
}
