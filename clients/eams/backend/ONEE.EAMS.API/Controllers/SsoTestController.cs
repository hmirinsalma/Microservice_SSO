using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONEE.EAMS.Application.Common;

namespace ONEE.EAMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SsoTestController : ControllerBase
{
    /// <summary>
    /// Endpoint de test pour vérifier l'intégration SSO et les custom claims EAMS
    /// </summary>
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        // ⭐ Récupérer les custom claims EAMS depuis HttpContext.Items
        var eamsUserId = HttpContext.Items["EamsUserId"]?.ToString();
        var serviceId = HttpContext.Items["ServiceId"]?.ToString();

        // Récupérer les claims standards du JWT
        var sub = User.FindFirst("sub")?.Value;
        var email = User.FindFirst("email")?.Value;
        var name = User.FindFirst("name")?.Value;
        
        // Récupérer les rôles
        var roles = User.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
            .Select(c => c.Value)
            .ToList();

        // Si pas de rôles avec ce claim type, essayer avec "role"
        if (!roles.Any())
        {
            roles = User.FindAll("role").Select(c => c.Value).ToList();
        }

        var profileData = new
        {
            sub,
            email,
            name,
            roles,
            customClaims = new
            {
                eamsUserId,
                serviceId
            },
            allClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
        };

        return Ok(ApiResponse<object>.Ok(profileData));
    }

    /// <summary>
    /// Endpoint de test avec autorisation par rôle
    /// </summary>
    [HttpGet("admin-only")]
    [Authorize(Roles = "Admin_Patrimoine")]
    public IActionResult AdminOnly()
    {
        var eamsUserId = HttpContext.Items["EamsUserId"]?.ToString();
        
        return Ok(ApiResponse<object>.Ok(new 
        { 
            message = "✅ Accès Admin_Patrimoine autorisé",
            eamsUserId 
        }));
    }

    /// <summary>
    /// Endpoint de test - simulation d'une requête équipements avec filtrage par service
    /// </summary>
    [HttpGet("equipments")]
    public IActionResult GetEquipments()
    {
        var eamsUserId = HttpContext.Items["EamsUserId"]?.ToString();
        var serviceId = HttpContext.Items["ServiceId"]?.ToString();
        var roles = User.FindAll("role").Select(c => c.Value).ToList();

        // Simulation de logique métier avec filtrage par périmètre
        var message = serviceId != null 
            ? $"📦 Équipements filtrés pour le service: {serviceId}"
            : "📦 Accès à tous les équipements";

        return Ok(ApiResponse<object>.Ok(new
        {
            message,
            eamsUserId,
            serviceId,
            roles,
            note = "Cet endpoint simule le filtrage RBAC basé sur les custom claims"
        }));
    }
}
