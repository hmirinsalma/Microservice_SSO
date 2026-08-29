using TIMS.API.Common;
using TIMS.API.DTOs.User;

namespace TIMS.API.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetAllAsync(int page, int pageSize);
    Task<UserDto> GetByIdAsync(int id);
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<UserDto> UpdateAsync(int id, UpdateUserDto dto);
    Task DeleteAsync(int id);
    Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileDto dto);
    Task UpdateProfilePhotoAsync(int userId, IFormFile file);
    Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
    Task<List<UserDto>> GetTechniciensByServiceAsync(int serviceId);
}
