using GestionPersonnel.API.Data;
using GestionPersonnel.API.Infrastructure;
using GestionPersonnel.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionPersonnel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;
    private readonly AppDbContext      _db;

    public DashboardController(IDashboardService service, AppDbContext db)
    {
        _service = service;
        _db      = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var role = ClaimsHelper.GetRole(User);

        return role switch
        {
            "AdministrateurRH" => Ok(await _service.GetAdminDashboardAsync()),
            "Directeur"        => await GetDirecteurDashboard(),
            "ChefDeService"    => await GetChefDashboard(),
            "Employe"          => await GetEmployeDashboard(),
            _                  => Forbid(),
        };
    }

    private async Task<IActionResult> GetDirecteurDashboard()
    {
        try
        {
            var userId  = await ClaimsHelper.ResolveLocalUserIdAsync(User, _db);
            var employe = await _db.Employes.FirstOrDefaultAsync(e => e.UserId == userId);
            if (employe == null)
                return Ok(new { message = "Aucune fiche employé liée à ce compte." });
            return Ok(await _service.GetDirecteurDashboardAsync(employe.DirectionId));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Ok(new { message = $"Erreur d'accès: {ex.Message}. Vérifiez que votre fiche employé est liée à votre compte SSO." });
        }
    }

    private async Task<IActionResult> GetChefDashboard()
    {
        try
        {
            var userId  = await ClaimsHelper.ResolveLocalUserIdAsync(User, _db);
            var employe = await _db.Employes.FirstOrDefaultAsync(e => e.UserId == userId);
            if (employe == null)
                return Ok(new { message = "Aucune fiche employé liée à ce compte." });
            return Ok(await _service.GetChefServiceDashboardAsync(employe.ServiceId));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Ok(new { message = $"Erreur d'accès: {ex.Message}. Vérifiez que votre fiche employé est liée à votre compte SSO." });
        }
    }

    private async Task<IActionResult> GetEmployeDashboard()
    {
        try
        {
            var userId  = await ClaimsHelper.ResolveLocalUserIdAsync(User, _db);
            var employe = await _db.Employes.FirstOrDefaultAsync(e => e.UserId == userId);
            if (employe == null)
                return Ok(new { message = "Aucune fiche employé liée à ce compte." });
            return Ok(await _service.GetEmployeDashboardAsync(employe.Id));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Ok(new { message = $"Erreur d'accès: {ex.Message}. Vérifiez que votre fiche employé est liée à votre compte SSO." });
        }
    }
}
