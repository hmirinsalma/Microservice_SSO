using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace ONEE.EAMS.Application.Services;

/// <summary>
/// Service pour envoyer des notifications au SSO
/// </summary>
public class SsoNotificationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SsoNotificationService> _logger;

    public SsoNotificationService(
        HttpClient httpClient, 
        IConfiguration configuration,
        ILogger<SsoNotificationService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendLoginNotificationAsync(string ssoUserId, string email, string ipAddress, string? userAgent)
    {
        try
        {
            var ssoApiUrl = _configuration["SsoSettings:ApiUrl"] ?? "http://localhost:5205/api";
            
            var payload = new
            {
                userId = ssoUserId,
                title = "Connexion réussie à EAMS",
                message = $"Vous vous êtes connecté avec succès à l'application EAMS.",
                type = "success",
                clientApplicationName = "EAMS",
                ipAddress = ipAddress,
                userAgent = userAgent
            };

            var response = await _httpClient.PostAsJsonAsync($"{ssoApiUrl}/Notifications/create", payload);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"✅ Notification SSO envoyée pour {email}");
            }
            else
            {
                _logger.LogWarning($"⚠️ Échec envoi notification SSO pour {email}: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Erreur lors de l'envoi de notification SSO pour {email}");
            // Ne pas bloquer le login si la notification échoue
        }
    }
}
