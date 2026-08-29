using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ONEE.EAMS.Application.Common;
using ONEE.EAMS.Application.DTOs.User;
using ONEE.EAMS.Application.Helpers;
using ONEE.EAMS.Application.Interfaces;
using ONEE.EAMS.Domain.Entities;

namespace ONEE.EAMS.Application.Services;

/// <summary>
/// Service de gestion des utilisateurs métier EAMS.
///
/// Responsabilités :
///   - CRUD des profils utilisateurs métier (nom, prénom, rôle, service, téléphone)
///   - Liaison avec les équipements et maintenances
///
/// Hors périmètre (délégué au microservice SSO) :
///   - Authentification
///   - Gestion des mots de passe
///   - Gestion des tokens
///   - Création de comptes d'authentification
/// </summary>
public class UserService : IUserService
{
    private readonly IAppDbContext _db;

    public UserService(IAppDbContext db) => _db = db;

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        return await _db.Users
            .Include(u => u.Service)
            .AsNoTracking()
            .OrderBy(u => u.Nom)
            .Select(u => MapDto(u))
            .ToListAsync();
    }

    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var u = await _db.Users.Include(x => x.Service).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException($"Utilisateur {id} introuvable.");
        return MapDto(u);
    }

    public async Task<UserDto> GetProfileAsync(ClaimsPrincipal user)
    {
        var userId = user.GetUserId();
        return await GetByIdAsync(userId);
    }

    /// <summary>
    /// Mise à jour du profil personnel — téléphone et photo uniquement.
    /// Le mot de passe est géré par le microservice SSO.
    /// </summary>
    public async Task<UserDto> UpdateProfileAsync(UpdateProfileRequest request, ClaimsPrincipal user)
    {
        var userId = user.GetUserId();
        var u = await _db.Users.Include(x => x.Service).FirstOrDefaultAsync(x => x.Id == userId)
            ?? throw new NotFoundException("Utilisateur introuvable.");

        if (request.Telephone != null) u.Telephone = request.Telephone;
        if (request.PhotoUrl != null)  u.PhotoUrl  = request.PhotoUrl;
        await _db.SaveChangesAsync();
        return MapDto(u);
    }

    /// <summary>
    /// Crée un profil utilisateur métier EAMS.
    /// Aucun compte d'authentification n'est créé ici — c'est la responsabilité du SSO.
    /// Le SsoId est optionnel pendant la phase de transition.
    /// </summary>
    public async Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            throw new ConflictException("Un utilisateur avec cet email existe déjà.");

        var u = new User
        {
            Id = Guid.NewGuid(),
            SsoId = request.SsoId,
            Nom = request.Nom,
            Prenom = request.Prenom,
            Email = request.Email,
            Telephone = request.Telephone,
            Poste = request.Poste,
            RoleMetier = request.Role,
            ServiceId = request.ServiceId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _db.Users.Add(u);
        await _db.SaveChangesAsync();
        await _db.Users.Entry(u).Reference(x => x.Service).LoadAsync();
        return MapDto(u);
    }

    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var u = await _db.Users.Include(x => x.Service).FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException($"Utilisateur {id} introuvable.");
        u.Nom      = request.Nom;
        u.Prenom   = request.Prenom;
        u.Telephone = request.Telephone;
        u.Poste    = request.Poste;
        u.RoleMetier = request.Role;
        u.ServiceId = request.ServiceId;
        u.IsActive = request.IsActive;
        await _db.SaveChangesAsync();
        return MapDto(u);
    }

    public async Task ToggleActiveAsync(Guid id)
    {
        var u = await _db.Users.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException($"Utilisateur {id} introuvable.");
        u.IsActive = !u.IsActive;
        await _db.SaveChangesAsync();
    }

    private static UserDto MapDto(User u) =>
        new(u.Id, u.Nom, u.Prenom, u.Email, u.Telephone, u.Poste,
            u.PhotoUrl, u.RoleMetier, u.ServiceId, u.Service?.Nom, u.IsActive);
}
