using Microsoft.EntityFrameworkCore;
using TIMS.API.Common;
using TIMS.API.Data;
using TIMS.API.Interfaces;

namespace TIMS.API.Services;

/// <summary>
/// ⚠️ SERVICE TEMPORAIRE — STUB UNIQUEMENT
///
/// Contient TOUTE la logique BCrypt/mot de passe.
/// Seul composant autorisé à accéder à StubCredentials pour les mots de passe.
///
/// SSO Migration :
///   1. Supprimer ce fichier
///   2. Supprimer IStubPasswordService
///   3. Supprimer le binding DI dans Program.cs
/// </summary>
public class StubPasswordService : IStubPasswordService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<StubPasswordService> _log;

    public StubPasswordService(ApplicationDbContext db, ILogger<StubPasswordService> log)
    { _db = db; _log = log; }

    public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var cred = await _db.StubCredentials.FirstOrDefaultAsync(c => c.UserId == userId)
            ?? throw new NotFoundException("Credentials introuvables");

        if (!BCrypt.Net.BCrypt.Verify(currentPassword, cred.PasswordHash))
            throw new AppException("Mot de passe actuel incorrect", 400, "INVALID_CURRENT_PASSWORD");

        cred.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword, 12);
        await _db.SaveChangesAsync();
        _log.LogInformation("[STUB] Mot de passe modifié pour userId={UserId}", userId);
    }

    public async Task<bool> VerifyPasswordAsync(int userId, string password)
    {
        var cred = await _db.StubCredentials.FirstOrDefaultAsync(c => c.UserId == userId);
        return cred != null && BCrypt.Net.BCrypt.Verify(password, cred.PasswordHash);
    }

    public async Task CreateCredentialAsync(int userId, string email, string password)
    {
        _db.StubCredentials.Add(new Entities.StubCredentials
        {
            UserId       = userId,
            Email        = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, 12)
        });
        await _db.SaveChangesAsync();
    }
}
