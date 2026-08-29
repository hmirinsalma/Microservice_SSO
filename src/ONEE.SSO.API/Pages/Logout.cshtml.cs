using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ONEE.SSO.API.Pages;

public class LogoutModel : PageModel
{
    public string? UserEmail { get; set; }

    public void OnGet()
    {
        // Récupérer l'email avant de supprimer la session
        UserEmail = HttpContext.Session.GetString("UserEmail");

        // Supprimer toutes les données de session
        HttpContext.Session.Clear();
    }

    public IActionResult OnPost()
    {
        // Supprimer la session
        HttpContext.Session.Clear();

        // Rediriger vers la page de login
        return RedirectToPage("/Login");
    }
}
