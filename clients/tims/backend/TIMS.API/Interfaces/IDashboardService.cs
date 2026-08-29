using TIMS.API.DTOs.Dashboard;

namespace TIMS.API.Interfaces;

public interface IDashboardService
{
    Task<AdminDashboardDto> GetAdminDashboardAsync();
    Task<DirecteurDashboardDto> GetDirecteurDashboardAsync();
    Task<ChefServiceDashboardDto> GetChefServiceDashboardAsync(int serviceId);
    Task<TechnicienDashboardDto> GetTechnicienDashboardAsync(int technicienId);
}
