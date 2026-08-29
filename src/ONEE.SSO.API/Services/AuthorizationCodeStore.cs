using System.Collections.Concurrent;

namespace ONEE.SSO.API.Services;

/// <summary>
/// Service singleton pour stocker les codes d'autorisation en mémoire
/// Partagé entre les Razor Pages et les Controllers API
/// </summary>
public class AuthorizationCodeStore
{
    private readonly ConcurrentDictionary<string, AuthorizationCodeData> _codes = new();
    private readonly ConcurrentDictionary<string, DateTime> _consumedCodes = new(); // Pour gérer les appels en double
    private readonly ILogger<AuthorizationCodeStore> _logger;

    public AuthorizationCodeStore(ILogger<AuthorizationCodeStore> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Stocker un code d'autorisation
    /// </summary>
    public void StoreCode(string code, string accessToken, string clientId, string userEmail)
    {
        var data = new AuthorizationCodeData
        {
            AccessToken = accessToken,
            ClientId = clientId,
            UserEmail = userEmail,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5) // Les codes expirent après 5 minutes
        };

        _codes[code] = data;
        _logger.LogInformation("Stored authorization code: {Code} for client: {ClientId}, user: {Email}", code, clientId, userEmail);
    }

    /// <summary>
    /// Récupérer un code d'autorisation (sans le supprimer - pour gérer les appels en double)
    /// Le code reste valide pendant 10 secondes pour permettre les retries
    /// </summary>
    public AuthorizationCodeData? ConsumeCode(string code)
    {
        // Vérifier si le code a déjà été consommé récemment (dans les 10 dernières secondes)
        if (_consumedCodes.TryGetValue(code, out var consumedAt))
        {
            if (DateTime.UtcNow - consumedAt < TimeSpan.FromSeconds(10))
            {
                _logger.LogWarning("⚠️ Authorization code already consumed recently (duplicate call detected): {Code}", code);
                
                // Retourner les données du code même s'il a déjà été consommé (pour gérer les appels en double)
                if (_codes.TryGetValue(code, out var cachedData))
                {
                    _logger.LogInformation("✅ Returning cached data for duplicate call: {Code}", code);
                    return cachedData;
                }
            }
            else
            {
                // Le code a été consommé il y a plus de 10 secondes, le nettoyer
                _consumedCodes.TryRemove(code, out _);
            }
        }

        // Première consommation du code
        if (_codes.TryGetValue(code, out var data))
        {
            // Vérifier si le code n'a pas expiré
            if (data.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Authorization code expired: {Code}", code);
                _codes.TryRemove(code, out _);
                return null;
            }

            // Marquer le code comme consommé (mais ne pas le supprimer tout de suite)
            _consumedCodes[code] = DateTime.UtcNow;
            _logger.LogInformation("✅ Consumed authorization code: {Code} for client: {ClientId}", code, data.ClientId);
            
            // Supprimer le code après 10 secondes (en arrière-plan)
            _ = Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith(_ => {
                _codes.TryRemove(code, out var _);
                _consumedCodes.TryRemove(code, out var _);
                _logger.LogInformation("🧹 Cleaned up authorization code after 10 seconds: {Code}", code);
            });
            
            return data;
        }

        _logger.LogWarning("❌ Authorization code not found: {Code}", code);
        return null;
    }

    /// <summary>
    /// Nettoyer les codes expirés (peut être appelé périodiquement)
    /// </summary>
    public void CleanupExpiredCodes()
    {
        var expiredCodes = _codes
            .Where(kvp => kvp.Value.ExpiresAt < DateTime.UtcNow)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var code in expiredCodes)
        {
            _codes.TryRemove(code, out _);
        }

        if (expiredCodes.Count > 0)
        {
            _logger.LogInformation("Cleaned up {Count} expired authorization codes", expiredCodes.Count);
        }
    }
}

/// <summary>
/// Données associées à un code d'autorisation
/// </summary>
public class AuthorizationCodeData
{
    public required string AccessToken { get; set; }
    public required string ClientId { get; set; }
    public required string UserEmail { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}
