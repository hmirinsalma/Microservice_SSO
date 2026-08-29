using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;
using ONEE.SSO.API.Services;

namespace ONEE.SSO.API.Pages.Connect;

public class AuthorizeModel : PageModel
{
    private readonly AuthorizationCodeStore _codeStore;

    public AuthorizeModel(AuthorizationCodeStore codeStore)
    {
        _codeStore = codeStore;
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

    public IActionResult OnGet()
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

        // L'utilisateur est authentifié, afficher la page de consentement
        return Page();
    }

    public IActionResult OnPost(string action)
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
