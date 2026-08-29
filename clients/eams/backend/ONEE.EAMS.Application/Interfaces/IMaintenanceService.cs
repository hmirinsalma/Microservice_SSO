using System.Security.Claims;
using ONEE.EAMS.Application.Common;
using ONEE.EAMS.Application.DTOs.Maintenance;

namespace ONEE.EAMS.Application.Interfaces;

public interface IMaintenanceService
{
    Task<PagedResult<MaintenanceListDto>> GetAllAsync(MaintenanceFilterRequest filter, ClaimsPrincipal user);
    Task<MaintenanceDetailDto> GetByIdAsync(Guid id, ClaimsPrincipal user);
    Task<MaintenanceDetailDto> CreateAsync(CreateMaintenanceRequest request, ClaimsPrincipal user);
    Task<MaintenanceDetailDto> UpdateAsync(Guid id, UpdateMaintenanceRequest request, ClaimsPrincipal user);
    Task<MaintenanceDetailDto> CloturerAsync(Guid id, CloturerMaintenanceRequest request, ClaimsPrincipal user);
    Task DeleteAsync(Guid id, ClaimsPrincipal user);
    Task UpdateRetardStatusAsync();
}
