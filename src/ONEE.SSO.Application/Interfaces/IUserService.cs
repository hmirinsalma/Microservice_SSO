using ONEE.SSO.Application.DTOs;

namespace ONEE.SSO.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();

    Task<UserDto?> GetByIdAsync(Guid id);

    Task<UserDto?> GetByEmailAsync(string email);

    Task<UserDto> CreateAsync(CreateUserDto dto);

    Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto);

    Task DeleteAsync(Guid id);

    Task<bool> ExistsByEmailAsync(string email);

    Task<IEnumerable<UserDto>> SearchAsync(string keyword);

    Task<IEnumerable<UserDto>> GetPagedAsync(int page, int pageSize);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);
    Task<IEnumerable<UserDto>> FilterAsync(
    string? firstName,
    string? lastName,
    bool? isActive);
}