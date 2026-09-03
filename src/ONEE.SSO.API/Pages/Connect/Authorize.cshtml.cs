using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;
using ONEE.SSO.API.Services;

namespace ONEE.SSO.API.Pages.Connect;

public class AuthorizeModel : PageModel
{
    private readonly AuthorizationCodeStore _codeStore;
    private readonly ONEE.SSO.Application.Repositories.IUserConsentRepository _consentRepository;
    private readonly ONEE.SSO.Application.Repositories.IUserRepository _userRepository;

    public AuthorizeModel(
        AuthorizationCodeStore codeStore,
        ONEE.SSO.Application.Repositories.IUserConsentRepository consentRepository,
        ONEE.SSO.Application.Repositories.IUserRepository userRepository)
    {
        _codeStore = codeStore;
        _consentRepository = consentRepository;
        _userRepository = userRepository;
    }
    // Paramètres OIDC de la requête
    [BindProperty(SupportsGet = true)]
    public string? client_id { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? redirect_uri { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? response_type { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? scope { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? state { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? code_challenge { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? code_challenge_method { get; set; }

    // Informations de l'application
    public string? ClientName { get; set; }
    public List<string> RequestedScopes { get; set; } = new();
    
    // Informations utilisateur
    public string? UserEmail { get; set; }
    public bool IsAuthenticated { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // Vérifier les paramètres obligatoires
        if (string.IsNullOrEmpty(client_id) || string.IsNullOrEmpty(redirect_uri))
        {
            return BadRequest("Paramètres client_id et redirect_uri obligatoires");
        }

        // Mapper le client_id vers un nom lisible
        ClientName = client_id switch
        {
            "gestion-personnel" => "Gestion Personnel",
            "tims-app" => "TIMS - Gestion des Interventions",
            "eams-spa" => "EAMS - Gestion des Équipements",
            _ => client_id
        };

        // Parser les scopes
        if (!string.IsNullOrEmpty(scope))
        {
            RequestedScopes = scope.Split(' ').ToList();
        }

        // Vérifier si l'utilisateur est déjà authentifié
        var accessToken = HttpContext.Session.GetString("AccessToken");
        UserEmail = HttpContext.Session.GetString("UserEmail");
        IsAuthenticated = !string.IsNullOrEmpty(accessToken);

        if (!IsAuthenticated)
        {
            // Rediriger vers la page de login avec les paramètres
            // Request.QueryString.Value commence par "?", il faut le garder pour reconstruire l'URL complète
            var returnUrl = $"/connect/authorize{Request.QueryString.Value}";
            var loginUrl = $"/Login?return_url={Uri.EscapeDataString(returnUrl)}&client_name={Uri.EscapeDataString(ClientName ?? "")}";
            return Redirect(loginUrl);
        }

        // ⭐ NOUVEAU: Vérifier si l'utilisateur a déjà donné son consentement (global ou spécifique à l'app)
        if (!string.IsNullOrEmpty(UserEmail))
        {
            Console.WriteLine($"[AUTHORIZE] Checking consent for {UserEmail} and {client_id}");
            var user = await _userRepository.GetByEmailAsync(UserEmail);
            
            if (user == null)
            {
                Console.WriteLine($"[AUTHORIZE] User not found in database: {UserEmail}");
            }
            else
            {
                Console.WriteLine($"[AUTHORIZE] User found: {user.Id}");
                
                if (!string.IsNullOrEmpty(client_id))
                {
                    // Vérifier si l'utilisateur a déjà consenti à AU MOINS UNE application (consentement global)
                    var allUserConsents = await _consentRepository.GetByUserIdAsync(user.Id);
                    var hasAnyConsent = allUserConsents.Any();
                    
                    Console.WriteLine($"[AUTHORIZE] User has {allUserConsents.Count()} total consents");
                    Console.WriteLine($"[AUTHORIZE] HasAnyConsent (global): {hasAnyConsent}");
                    
                    if (hasAnyConsent)
                    {
                        // L'utilisateur a déjà consenti à au moins une app, on skip la page d'autorisation
                        Console.WriteLine($"[AUTHORIZE] User {UserEmail} has already consented to at least one app, skipping consent page for {client_id}");
                        
                        var authorizationCode = GenerateAuthorizationCode();
                        Console.WriteLine($"[AUTHORIZE] Generated authorization code: {authorizationCode} for {client_id}");
                        
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            _codeStore.StoreCode(authorizationCode, accessToken, client_id, UserEmail);
                        }
                        
                        // Rediriger directement vers le callback
                        var callbackUrl = $"{redirect_uri}?code={authorizationCode}";
                        if (!string.IsNullOrEmpty(state))
                        {
                            callbackUrl += $"&state={state}";
                        }
                        
                        return Redirect(callbackUrl);
                    }
                    else
                    {
                        Console.WriteLine($"[AUTHORIZE] User {UserEmail} has NO consents yet, showing consent page");
                    }
                }
            }
        }

        // L'utilisateur est authentifié mais n'a pas encore consenti, afficher la page de consentement
        Console.WriteLine($"[AUTHORIZE] User {UserEmail} needs to consent to {client_id}");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string action)
    {
        // Vérifier que l'utilisateur est authentifié
        var accessToken = HttpContext.Session.GetString("AccessToken");
        if (string.IsNullOrEmpty(accessToken))
        {
            return RedirectToPage("/Login");
        }

        if (action == "deny")
        {
            // L'utilisateur refuse l'autorisation
            var errorUrl = $"{redirect_uri}?error=access_denied&error_description=User denied access&state={state}";
            return Redirect(errorUrl);
        }

        // L'utilisateur accepte, générer un code d'autorisation
        var authorizationCode = GenerateAuthorizationCode();
        
        // Récupérer l'email de l'utilisateur depuis la session
        var userEmail = HttpContext.Session.GetString("UserEmail");
        
        // LOG: Code généré
        Console.WriteLine($"[AUTHORIZE] Generated authorization code: {authorizationCode} (length={authorizationCode.Length})");
        Console.WriteLine($"[AUTHORIZE] For client_id: {client_id}, user: {userEmail}");
        Console.WriteLine($"[AUTHORIZE] Will redirect to: {redirect_uri}?code={authorizationCode}");
        
        // Stocker le code dans le AuthorizationCodeStore (singleton partagé)
        _codeStore.StoreCode(authorizationCode, accessToken, client_id ?? "", userEmail ?? "");
        
        Console.WriteLine($"[AUTHORIZE] Stored in AuthorizationCodeStore");
        
        // ⭐ NOUVEAU: Enregistrer le consentement dans la base de données
        if (!string.IsNullOrEmpty(userEmail))
        {
            var user = await _userRepository.GetByEmailAsync(userEmail);
            if (user != null && !string.IsNullOrEmpty(client_id))
            {
                try
                {
                    var existingConsent = await _consentRepository.GetByUserAndClientAsync(user.Id, client_id);
                    if (existingConsent == null)
                    {
                        var consent = new ONEE.SSO.Domain.Entities.UserConsent
                        {
                            Id = Guid.NewGuid(),
                            UserId = user.Id,
                            ClientId = client_id,
                            Scopes = scope ?? "openid profile email",
                            GrantedAt = DateTime.UtcNow,
                            ExpiresAt = null, // Consentement permanent
                            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                        };
                        
                        await _consentRepository.AddAsync(consent);
                        await _consentRepository.SaveChangesAsync();
                        Console.WriteLine($"[AUTHORIZE] Consent saved for user {userEmail} and app {client_id}");
                    }
                }
                catch (Exception ex)
                {
                    // Logger l'erreur mais ne pas bloquer l'autorisation
                    Console.WriteLine($"[AUTHORIZE] Error saving consent: {ex.Message}");
                }
            }
        }
        
        // Construire l'URL de redirection avec le code
        var callbackUrl = $"{redirect_uri}?code={authorizationCode}";
        
        if (!string.IsNullOrEmpty(state))
        {
            callbackUrl += $"&state={state}";
        }

        return Redirect(callbackUrl);
    }

    private string GenerateAuthorizationCode()
    {
        // Générer un code aléatoire sécurisé
        var bytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}
