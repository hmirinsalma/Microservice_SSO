using GestionPersonnel.API.DTOs.Dashboard;

namespace GestionPersonnel.API.Services.Interfaces;

public interface IDashboardService
{
    Task<AdminDashboardDto>      GetAdminDashboardAsync();
    Task<DirecteurDashboardDto>  GetDirecteurDashboardAsync(int directionId);
    Task<ChefServiceDashboardDto>GetChefServiceDashboardAsync(int serviceId);
    Task<EmployeDashboardDto>    GetEmployeDashboardAsync(int employeId);
}
