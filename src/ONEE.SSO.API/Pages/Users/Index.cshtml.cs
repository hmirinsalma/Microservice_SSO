using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.Domain.Entities;
using ONEE.SSO.API.Authorization;

namespace ONEE.SSO.API.Pages.Users;

[SsoAdminRequired]
public class IndexModel : PageModel
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public IndexModel(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
    }

    public List<UserDto> Users { get; set; } = new();
    public List<Role> AvailableRoles { get; set; } = new();
    
    public string? SearchTerm { get; set; }
    public Guid? SelectedRoleId { get; set; }
    public string? SelectedStatus { get; set; }
    
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalPages { get; set; }

    public async Task OnGetAsync(string? search, Guid? role, string? status, int page = 1)
    {
        SearchTerm = search;
        SelectedRoleId = role;
        SelectedStatus = status;
        CurrentPage = page;

        // Load available roles for filter
        AvailableRoles = (await _roleRepository.GetAllAsync()).ToList();

        // Get all users
        var allUsers = await _userRepository.GetAllAsync();
        
        // Apply filters
        var query = allUsers.AsQueryable();

        if (!string.IsNullOrEmpty(SearchTerm))
        {
            query = query.Where(u => 
                u.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.FirstName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.LastName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (SelectedStatus == "active")
            query = query.Where(u => u.IsActive);
        else if (SelectedStatus == "inactive")
            query = query.Where(u => !u.IsActive);

        var filteredUsers = query.ToList();

        // Filter by role
        if (SelectedRoleId.HasValue)
        {
            var userIdsWithRole = (await _userRoleRepository.GetByRoleIdAsync(SelectedRoleId.Value))
                .Select(ur => ur.UserId)
                .ToHashSet();
            
            filteredUsers = filteredUsers.Where(u => userIdsWithRole.Contains(u.Id)).ToList();
        }

        // Pagination
        TotalPages = (int)Math.Ceiling(filteredUsers.Count / (double)PageSize);
        var pagedUsers = filteredUsers
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        // Map to DTOs with roles
        Users = new List<UserDto>();
        foreach (var user in pagedUsers)
        {
            var userRoles = await _userRoleRepository.GetByUserIdAsync(user.Id);
            Users.Add(new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                SsoId = user.Id.ToString(),
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Roles = userRoles.Select(ur => ur.Role.Name).ToList()
            });
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        _userRepository.Delete(user);
        await _userRepository.SaveChangesAsync();
        
        TempData["SuccessMessage"] = "Utilisateur supprimé avec succès";
        return RedirectToPage();
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? SsoId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
