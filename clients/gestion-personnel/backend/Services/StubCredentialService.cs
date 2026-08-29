using GestionPersonnel.API.Data;
using GestionPersonnel.API.Models;
using GestionPersonnel.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionPersonnel.API.Services;

/// <summary>
/// TEMPORAIRE — Implémentation de IStubCredentialService.
/// Gère les hash BCrypt dans la table StubCredentials.
///
/// Toute référence à BCrypt est confinée dans ce fichier.
/// Aucun autre Service, Controller ou Repository ne doit utiliser BCrypt.
///
/// Supprimé lors de l'intégration SSO :
///   - Retirer l'enregistrement DI dans Program.cs
///   - Supprimer la table StubCredentials via une migration EF
///   - Supprimer ce fichier et StubCredential.cs
/// </summary>
public class StubCredentialService : IStubCredentialService
{
    private readonly AppDbContext _db;
    public StubCredentialService(AppDbContext db) => _db = db;

    public async Task CreateAsync(int userId, string plainPassword)
    {
        // Éviter les doublons
        var existing = await _db.StubCredentials.FirstOrDefaultAsync(c => c.UserId == userId);
        if (existing != null)
        {
            existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }
        else
        {
            _db.StubCredentials.Add(new StubCredential
            {
                UserId       = userId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword),
            });
        }
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int userId)
    {
        var cred = await _db.StubCredentials.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cred != null)
        {
            _db.StubCredentials.Remove(cred);
            await _db.SaveChangesAsync();
        }
    }
}
