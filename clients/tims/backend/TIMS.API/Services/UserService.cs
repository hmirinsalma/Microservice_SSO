using Microsoft.EntityFrameworkCore;
using TIMS.API.Common;
using TIMS.API.Data;
using TIMS.API.DTOs.User;
using TIMS.API.Entities;
using TIMS.API.Interfaces;

namespace TIMS.API.Services;

/// <summary>
/// Service métier utilisateurs — 100% SSO-Ready.
/// Ne connaît pas : BCrypt, PasswordHash, StubCredentials, JWT, StubAuthService.
/// La logique de mot de passe est déléguée à IStubPasswordService (temporaire).
/// </summary>
public class UserService : IUserService
{
    private readonly ApplicationDbContext  _db;
    private readonly IWebHostEnvironment   _env;
    private readonly IStubPasswordService  _pwdSvc; // ⚠️ STUB — supprimer lors SSO

    public UserService(ApplicationDbContext db, IWebHostEnvironment env, IStubPasswordService pwdSvc)
    { _db = db; _env = env; _pwdSvc = pwdSvc; }

    public async Task<PagedResult<UserDto>> GetAllAsync(int page, int pageSize)
    {
        var q = _db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.Service).Include(u => u.Equipe)
            .OrderBy(u => u.LastName);
        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<UserDto> { Items = items.Select(Map).ToList(), TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<UserDto> GetByIdAsync(int id)
    {
        var u = await _db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.Service).Include(u => u.Equipe)
            .FirstOrDefaultAsync(u => u.Id == id)
            ?? throw new NotFoundException("Utilisateur introuvable");
        return Map(u);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email.ToLower()))
            throw new ConflictException("Email déjà utilisé", "EMAIL_ALREADY_EXISTS");

        var user = new User
        {
            FirstName = dto.FirstName, LastName = dto.LastName,
            Email = dto.Email.ToLower(), Phone = dto.Phone, Poste = dto.Poste,
            ServiceId = dto.ServiceId, EquipeId = dto.EquipeId,
            RoleMetier = dto.RoleMetier
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Rôle d'autorisation JWT
        _db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = dto.RoleId });

        // ⚠️ STUB : créer les credentials temporaires via IStubPasswordService
        // BCrypt est 100% isolé dans StubPasswordService — UserService ne l'appelle jamais
        if (!string.IsNullOrEmpty(dto.Password))
            await _pwdSvc.CreateCredentialAsync(user.Id, user.Email, dto.Password);

        await _db.SaveChangesAsync();
        return await GetByIdAsync(user.Id);
    }

    public async Task<UserDto> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _db.Users.Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id) ?? throw new NotFoundException("Utilisateur introuvable");

        if (dto.ServiceId.HasValue) user.ServiceId = dto.ServiceId;
        if (dto.EquipeId.HasValue)  user.EquipeId  = dto.EquipeId;
        if (dto.IsActive.HasValue)  user.IsActive   = dto.IsActive.Value;
        if (!string.IsNullOrEmpty(dto.Poste)) user.Poste = dto.Poste;
        if (!string.IsNullOrEmpty(dto.RoleMetier)) user.RoleMetier = dto.RoleMetier;

        if (dto.RoleId.HasValue)
        {
            _db.UserRoles.RemoveRange(user.UserRoles);
            _db.UserRoles.Add(new UserRole { UserId = id, RoleId = dto.RoleId.Value });
        }
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var hasInterventions = await _db.Interventions.AnyAsync(i =>
            (i.ResponsableId == id || i.TechnicienId == id || i.ChefServiceId == id) && !i.IsDeleted);
        if (hasInterventions)
            throw new ConflictException("Utilisateur lié à des interventions", "USER_HAS_INTERVENTIONS");

        var user = await _db.Users.FindAsync(id) ?? throw new NotFoundException("Utilisateur introuvable");
        user.IsActive = false;
        await _db.SaveChangesAsync();
    }

    public async Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileDto dto)
    {
        var user = await _db.Users.FindAsync(userId) ?? throw new NotFoundException("Utilisateur introuvable");
        if (dto.Phone != null) user.Phone = dto.Phone;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return await GetByIdAsync(userId);
    }

    public async Task UpdateProfilePhotoAsync(int userId, IFormFile file)
    {
        var allowed = new[] { "image/jpeg","image/png","image/webp" };
        if (!allowed.Contains(file.ContentType))
            throw new AppException("Type de fichier non autorisé", 415, "INVALID_FILE_TYPE");
        if (file.Length > 2 * 1024 * 1024)
            throw new AppException("Photo trop volumineuse (max 2 Mo)", 413, "PROFILE_PHOTO_TOO_LARGE");

        var user = await _db.Users.FindAsync(userId) ?? throw new NotFoundException("Utilisateur introuvable");
        var dir  = Path.Combine(_env.ContentRootPath, "Uploads", "profiles");
        Directory.CreateDirectory(dir);
        var stored = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        await using var s = new FileStream(Path.Combine(dir, stored), FileMode.Create);
        await file.CopyToAsync(s);
        user.ProfilePhotoPath = $"/uploads/profiles/{stored}";
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// ⚠️ TEMPORAIRE STUB — Délègue à IStubPasswordService (tout BCrypt est là-bas).
    /// Sera supprimé lors de l'intégration SSO.
    /// </summary>
    public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
        => await _pwdSvc.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);

    public async Task<List<UserDto>> GetTechniciensByServiceAsync(int serviceId)
    {
        var users = await _db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.Service).Include(u => u.Equipe)
            .Where(u => u.ServiceId == serviceId && u.IsActive &&
                        u.UserRoles.Any(ur => ur.Role.Name == RoleNames.Technicien))
            .ToListAsync();
        return users.Select(Map).ToList();
    }

    private static UserDto Map(User u) => new()
    {
        Id = u.Id, FirstName = u.FirstName, LastName = u.LastName, Email = u.Email,
        Phone = u.Phone, Poste = u.Poste, ProfilePhotoPath = u.ProfilePhotoPath,
        IsActive = u.IsActive, CreatedAt = u.CreatedAt,
        ServiceId = u.ServiceId, ServiceName = u.Service?.Name,
        EquipeId  = u.EquipeId,  EquipeName  = u.Equipe?.Name,
        Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
    };
}
