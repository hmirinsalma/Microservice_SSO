using System.Security.Claims;
using ONEE.EAMS.Application.DTOs.Dashboard;

namespace ONEE.EAMS.Application.Interfaces;

public interface IDashboardService
{
    Task<object> GetDashboardAsync(ClaimsPrincipal user);
}
