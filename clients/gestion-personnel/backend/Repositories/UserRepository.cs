using GestionPersonnel.API.Data;
using GestionPersonnel.API.Models;
using GestionPersonnel.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionPersonnel.API.Repositories;

/// <summary>
/// Repository utilisé par StubAuthService pour résoudre un User à partir de son email.
/// Aucun champ de mot de passe — les credentials sont dans StubCredentials.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) => _context = context;

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>
                u.Email.ToLower() == email.ToLower() && u.IsActive);

    public async Task<User?> GetByIdAsync(int id)
        => await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

    public async Task<User?> GetBySsoIdAsync(string ssoId)
        => await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.SsoId == ssoId && u.IsActive);
}
