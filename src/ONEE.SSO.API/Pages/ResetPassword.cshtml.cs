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
    public string? Token { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Email { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Le nouveau mot de passe est obligatoire")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères, une majuscule, une minuscule, un chiffre et un caractère spécial")]
    public string NewPassword { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "La confirmation est obligatoire")]
    [Compare("NewPassword", ErrorMessage = "Les mots de passe ne correspondent pas")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    public bool IsTokenInvalid { get; set; }

    public void OnGet()
    {
        if (string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(Email))
        {
            IsTokenInvalid = true;
            ErrorMessage = "Le lien de réinitialisation est invalide ou a expiré.";
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(Email))
        {
            IsTokenInvalid = true;
            ErrorMessage = "Le lien de réinitialisation est invalide ou a expiré.";
            return Page();
        }

        if (!ModelState.IsValid)
        {
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
                SuccessMessage = "Votre mot de passe a été réinitialisé avec succès. Vous pouvez maintenant vous connecter.";
                // Vider les champs
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                
                if (errorContent.Contains("expiré") || errorContent.Contains("expired"))
                {
                    ErrorMessage = "Le lien de réinitialisation a expiré. Veuillez faire une nouvelle demande.";
                    IsTokenInvalid = true;
                }
                else if (errorContent.Contains("invalide") || errorContent.Contains("invalid"))
                {
                    ErrorMessage = "Le lien de réinitialisation est invalide.";
                    IsTokenInvalid = true;
                }
                else
                {
                    ErrorMessage = "Une erreur s'est produite lors de la réinitialisation. Veuillez réessayer.";
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur lors de la réinitialisation : {ex.Message}";
        }

        return Page();
    }
}
