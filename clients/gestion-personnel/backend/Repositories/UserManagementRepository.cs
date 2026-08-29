using GestionPersonnel.API.Data;
using GestionPersonnel.API.Models;
using GestionPersonnel.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionPersonnel.API.Repositories;

public class UserManagementRepository : IUserManagementRepository
{
    private readonly AppDbContext _context;
    public UserManagementRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<User>> GetAllAsync()
        => await _context.Users
            .Include(u => u.Role)
            .OrderBy(u => u.Role.Id)
            .ThenBy(u => u.Username)
            .ToListAsync();

    public async Task<User?> GetByIdAsync(int id)
        => await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

    public async Task<User?> GetByUsernameAsync(string username)
        => await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

    public async Task<IEnumerable<Role>> GetAllRolesAsync()
        => await _context.Roles.OrderBy(r => r.Id).ToListAsync();

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}
