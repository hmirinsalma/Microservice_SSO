using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            IsActive = user.IsActive
        };
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            return null;

        return MapToDto(user);
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
            return null;

        return MapToDto(user);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _userRepository.EmailExistsAsync(email);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = _passwordHasher.Hash(dto.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new Exception("User not found.");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;
        user.IsActive = dto.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new Exception("User not found.");

        _userRepository.Delete(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task ActivateAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new Exception("User not found.");

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new Exception("User not found.");

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
    }
    public async Task<IEnumerable<UserDto>> SearchAsync(string keyword)
    {
        var users = await _userRepository.FindAsync(u =>
            u.FirstName.Contains(keyword) ||
            u.LastName.Contains(keyword) ||
            u.Email.Contains(keyword));

        return users.Select(MapToDto);
    }

    public async Task<IEnumerable<UserDto>> GetPagedAsync(int page, int pageSize)
    {
        var users = await _userRepository.GetAllAsync();

        return users
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
           .Select(MapToDto);
    }
    public async Task<IEnumerable<UserDto>> FilterAsync(
    string? firstName,
    string? lastName,
    bool? isActive)
    {
        var users = await _userRepository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(firstName))
        {
            users = users.Where(u =>
                u.FirstName.Contains(firstName,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(lastName))
        {
            users = users.Where(u =>
                u.LastName.Contains(lastName,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (isActive.HasValue)
        {
            users = users.Where(u => u.IsActive == isActive.Value);
        }

        return users.Select(MapToDto);
    }
}