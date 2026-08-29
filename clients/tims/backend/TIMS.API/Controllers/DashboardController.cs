using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIMS.API.Common;
using TIMS.API.Data;
using TIMS.API.DTOs.Dashboard;
using TIMS.API.Entities;
using TIMS.API.Extensions;
using TIMS.API.Interfaces;

namespace TIMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService    _svc;
    private readonly ApplicationDbContext _db;

    public DashboardController(IDashboardService svc, ApplicationDbContext db)
    { _svc = svc; _db = db; }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        // Autorisation via claim JWT (jamais lecture BDD)
        var role   = ClaimsHelper.GetRole(User);
        var userId = ClaimsHelper.GetTimsUserId(User);
        var svcId  = ClaimsHelper.GetServiceId(User);

        return role switch
        {
            RoleNames.AdminTechnique    => Ok(ApiResponse<AdminDashboardDto>.Ok(await _svc.GetAdminDashboardAsync())),
            RoleNames.DirecteurTechnique=> Ok(ApiResponse<DirecteurDashboardDto>.Ok(await _svc.GetDirecteurDashboardAsync())),
            RoleNames.ChefService when svcId.HasValue
                                        => Ok(ApiResponse<ChefServiceDashboardDto>.Ok(await _svc.GetChefServiceDashboardAsync(svcId.Value))),
            RoleNames.Technicien        => Ok(ApiResponse<TechnicienDashboardDto>.Ok(await _svc.GetTechnicienDashboardAsync(userId))),
            _                           => Forbid()
        };
    }
}
