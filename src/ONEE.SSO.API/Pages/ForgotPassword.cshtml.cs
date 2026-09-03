using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace ONEE.SSO.API.Pages;

public class ForgotPasswordModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public ForgotPasswordModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [BindProperty]
    [Required(ErrorMessage = "L'adresse email est obligatoire")]
    [EmailAddress(ErrorMessage = "L'adresse email n'est pas valide")]
    public string Email { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    public bool ShowSuccessMessage { get; set; }

    public void OnGet()
    {
        // Page initiale
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Veuillez entrer une adresse email valide.";
            return Page();
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["BaseUrl"] ?? "http://localhost:5205";
            
            var request = new
            {
                email = Email
            };

            var jsonContent = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{baseUrl}/api/Auth/forgot-password", httpContent);

            if (response.IsSuccessStatusCode)
            {
                ShowSuccessMessage = true;
                SuccessMessage = "Un email de réinitialisation a été envoyé à votre adresse. Veuillez vérifier votre boîte de réception.";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Pour des raisons de sécurité, on affiche le même message même si l'email n'existe pas
                ShowSuccessMessage = true;
                SuccessMessage = "Si cette adresse email existe dans notre système, un email de réinitialisation a été envoyé.";
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
