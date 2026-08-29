using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace ONEE.SSO.API.Pages;

public class LoginModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public LoginModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [BindProperty]
    [Required(ErrorMessage = "L'adresse email est obligatoire")]
    [EmailAddress(ErrorMessage = "L'adresse email n'est pas valide")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Le mot de passe est obligatoire")]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public bool RememberMe { get; set; }

    public string? ErrorMessage { get; set; }
    public string? ClientName { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public void OnGet(string? client_name, string? return_url)
    {
        ClientName = client_name;
        ReturnUrl = return_url;
    }

    public async Task<IActionResult> OnPostAsync(string? return_url)
    {
        // Debug: Si ReturnUrl n'est pas bindé via la propriété, le récupérer du paramètre
        if (string.IsNullOrEmpty(ReturnUrl) && !string.IsNullOrEmpty(return_url))
        {
            ReturnUrl = return_url;
        }

        Console.WriteLine($"[LOGIN POST] ReturnUrl = {ReturnUrl}");

        if (!ModelState.IsValid)
        {
            ErrorMessage = "Veuillez remplir tous les champs correctement.";
            return Page();
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["BaseUrl"] ?? "http://localhost:5205";
            
            var loginRequest = new
            {
                email = Email,
                password = Password
            };

            var jsonContent = JsonSerializer.Serialize(loginRequest);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{baseUrl}/api/Auth/login", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (loginResponse != null)
                {
                    // Stocker le token dans une session ou cookie
                    HttpContext.Session.SetString("AccessToken", loginResponse.AccessToken);
                    HttpContext.Session.SetString("RefreshToken", loginResponse.RefreshToken);
                    HttpContext.Session.SetString("UserEmail", Email);

                    Console.WriteLine($"[LOGIN SUCCESS] Redirecting to: {ReturnUrl ?? "/Dashboard"}");

                    // Rediriger vers le dashboard ou l'URL de retour
                    if (!string.IsNullOrEmpty(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }

                    return RedirectToPage("/Dashboard");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ErrorMessage = "Email ou mot de passe incorrect.";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                
                // Vérifier si c'est un compte verrouillé
                if (errorContent.Contains("verrouillé") || errorContent.Contains("locked"))
                {
                    ErrorMessage = "Votre compte a été verrouillé suite à plusieurs tentatives de connexion échouées. Veuillez contacter un administrateur.";
                }
                else if (errorContent.Contains("désactivé") || errorContent.Contains("inactive"))
                {
                    ErrorMessage = "Votre compte a été désactivé. Veuillez contacter un administrateur.";
                }
                else
                {
                    ErrorMessage = "Une erreur s'est produite lors de la connexion. Veuillez réessayer.";
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur de connexion au serveur : {ex.Message}";
        }

        return Page();
    }

    private class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; } = string.Empty;
    }
}
