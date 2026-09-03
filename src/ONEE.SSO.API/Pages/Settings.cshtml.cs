using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using ONEE.SSO.API.Authorization;

namespace ONEE.SSO.API.Pages;

[SsoAdminRequired]
public class SettingsModel : PageModel
{
    private readonly IConfiguration _configuration;

    public SettingsModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // General Settings
    public string OrganizationName { get; set; } = "ONEE";
    public string LogoUrl { get; set; } = "/images/onee-logo.png";
    public string DefaultLanguage { get; set; } = "fr";
    public string Timezone { get; set; } = "Africa/Casablanca";

    // Security Settings
    public int AccessTokenLifetime { get; set; } = 60;
    public int RefreshTokenLifetime { get; set; } = 30;
    public int AuthCodeLifetime { get; set; } = 300;
    public int MaxLoginAttempts { get; set; } = 5;
    public bool RequireEmailVerification { get; set; } = false;
    public bool EnableTwoFactor { get; set; } = false;

    // Email Settings
    public string SmtpServer { get; set; } = "smtp.onee.ma";
    public int SmtpPort { get; set; } = 587;
    public string SmtpUsername { get; set; } = "";
    public string SmtpPassword { get; set; } = "";
    public string FromEmail { get; set; } = "noreply@onee.ma";
    public string FromName { get; set; } = "ONEE SSO";
    public bool EnableSsl { get; set; } = true;

    // Advanced Settings
    public string JwtSecretKey { get; set; } = "";
    public string CorsOrigins { get; set; } = "";
    public bool EnableAuditLogs { get; set; } = true;
    public bool EnableDebugMode { get; set; } = false;

    public void OnGet()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        // Load JWT settings
        var jwtSection = _configuration.GetSection("Jwt");
        JwtSecretKey = jwtSection["SecretKey"] ?? "";
        
        if (int.TryParse(jwtSection["AccessTokenExpirationMinutes"], out var accessMinutes))
        {
            AccessTokenLifetime = accessMinutes;
        }

        // Load CORS settings
        var corsOrigins = _configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        if (corsOrigins != null && corsOrigins.Length > 0)
        {
            CorsOrigins = string.Join("\n", corsOrigins);
        }

        // Mock data for other settings (à implémenter avec vraie source de config)
        OrganizationName = "ONEE - Office National de l'Electricité et de l'Eau potable";
        LogoUrl = "/images/onee-logo.png";
        
        RefreshTokenLifetime = 30;
        AuthCodeLifetime = 300;
        MaxLoginAttempts = 5;
        RequireEmailVerification = false;
        EnableTwoFactor = false;

        SmtpServer = "smtp.onee.ma";
        SmtpPort = 587;
        SmtpUsername = "noreply@onee.ma";
        FromEmail = "noreply@onee.ma";
        FromName = "ONEE SSO";
        EnableSsl = true;

        EnableAuditLogs = true;
        
        // Check if debug mode is enabled based on log level
        var logLevel = _configuration.GetValue<string>("Logging:LogLevel:Default");
        EnableDebugMode = logLevel?.Equals("Debug", StringComparison.OrdinalIgnoreCase) == true;
    }

    public IActionResult OnPostSaveGeneral(
        string organizationName,
        string logoUrl,
        string defaultLanguage,
        string timezone)
    {
        // TODO: Implémenter la sauvegarde dans appsettings.json ou base de données
        Console.WriteLine($"[SETTINGS] SaveGeneral: {organizationName}, {logoUrl}, {defaultLanguage}, {timezone}");
        
        TempData["SuccessMessage"] = "Paramètres généraux enregistrés avec succès";
        return RedirectToPage();
    }

    public IActionResult OnPostSaveSecurity(
        int accessTokenLifetime,
        int refreshTokenLifetime,
        int authCodeLifetime,
        int maxLoginAttempts,
        bool requireEmailVerification,
        bool enableTwoFactor)
    {
        // TODO: Implémenter la sauvegarde
        Console.WriteLine($"[SETTINGS] SaveSecurity: AccessToken={accessTokenLifetime}min, RefreshToken={refreshTokenLifetime}days");
        
        TempData["SuccessMessage"] = "Paramètres de sécurité enregistrés avec succès";
        return RedirectToPage();
    }

    public IActionResult OnPostSaveEmail(
        string smtpServer,
        int smtpPort,
        string smtpUsername,
        string smtpPassword,
        string fromEmail,
        string fromName,
        bool enableSsl)
    {
        // TODO: Implémenter la sauvegarde
        Console.WriteLine($"[SETTINGS] SaveEmail: {smtpServer}:{smtpPort}, From={fromEmail}");
        
        TempData["SuccessMessage"] = "Configuration email enregistrée avec succès";
        return RedirectToPage();
    }

    public IActionResult OnPostSaveAdvanced(
        string jwtSecretKey,
        string corsOrigins,
        bool enableAuditLogs,
        bool enableDebugMode)
    {
        // TODO: Implémenter la sauvegarde
        Console.WriteLine($"[SETTINGS] SaveAdvanced: AuditLogs={enableAuditLogs}, Debug={enableDebugMode}");
        
        TempData["SuccessMessage"] = "Paramètres avancés enregistrés avec succès";
        return RedirectToPage();
    }
}
