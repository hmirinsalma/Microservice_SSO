using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace ONEE.SSO.API.Pages;

public class ResetPasswordModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public ResetPasswordModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [BindProperty(SupportsGet = true)]
    public string Token { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Le mot de passe est obligatoire")]
    [MinLength(8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
        ErrorMessage = "Le mot de passe doit contenir au moins une majuscule, une minuscule, un chiffre et un caractère spécial")]
    public string NewPassword { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "La confirmation du mot de passe est obligatoire")]
    [Compare(nameof(NewPassword), ErrorMessage = "Les mots de passe ne correspondent pas")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    public bool ShowSuccessMessage { get; set; }

    public void OnGet(string token, string email)
    {
        Token = token;
        Email = email;

        if (string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(Email))
        {
            ErrorMessage = "Le lien de réinitialisation est invalide ou a expiré.";
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Veuillez remplir tous les champs correctement.";
            return Page();
        }

        if (string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(Email))
        {
            ErrorMessage = "Le lien de réinitialisation est invalide ou a expiré.";
            return Page();
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["BaseUrl"] ?? "http://localhost:5205";
            
            var request = new
            {
                email = Email,
                token = Token,
                newPassword = NewPassword
            };

            var jsonContent = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{baseUrl}/api/Auth/reset-password", httpContent);

            if (response.IsSuccessStatusCode)
            {
                ShowSuccessMessage = true;
                SuccessMessage = "Votre mot de passe a été réinitialisé avec succès. Vous pouvez maintenant vous connecter avec votre nouveau mot de passe.";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                
                if (errorContent.Contains("expired") || errorContent.Contains("expiré"))
                {
                    ErrorMessage = "Le lien de réinitialisation a expiré. Veuillez faire une nouvelle demande.";
                }
                else if (errorContent.Contains("invalid") || errorContent.Contains("invalide"))
                {
                    ErrorMessage = "Le lien de réinitialisation est invalide.";
                }
                else
                {
                    ErrorMessage = "Une erreur s'est produite. Veuillez réessayer.";
                }
            }
            else
            {
                ErrorMessage = "Une erreur s'est produite. Veuillez réessayer plus tard.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur de connexion au serveur : {ex.Message}";
        }

        return Page();
    }
}
