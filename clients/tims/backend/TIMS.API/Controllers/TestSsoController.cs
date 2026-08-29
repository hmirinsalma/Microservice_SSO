using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIMS.API.Common;

namespace TIMS.API.Controllers;

/// <summary>
/// Controller de test pour vérifier l'intégration SSO et les custom claims TIMS
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TestSsoController : ControllerBase
{
    /// <summary>
    /// Endpoint de test pour vérifier les custom claims TIMS depuis le JWT
    /// </summary>
    [HttpGet("verify-claims")]
    public IActionResult VerifyClaims()
    {
        // ⭐ Accéder aux custom claims TIMS depuis HttpContext.Items (via middleware)
        var timsUserId = HttpContext.Items["TimsUserId"]?.ToString();
        var timsServiceId = HttpContext.Items["TimsServiceId"]?.ToString();
        var timsTeamId = HttpContext.Items["TimsTeamId"]?.ToString();

        // Claims standards
        var email = User.FindFirst("email")?.Value;
        var sub = User.FindFirst("sub")?.Value;
        var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        // Tous les claims disponibles
        var allClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

        return Ok(ApiResponse<object>.Ok(new
        {
            message = "✅ SSO Integration successful",
            customClaims = new
            {
                tims_user_id = timsUserId,
                tims_service_id = timsServiceId,
                tims_team_id = timsTeamId
            },
            standardClaims = new
            {
                email,
                sub,
                roles
            },
            allClaims
        }));
    }

    /// <summary>
    /// Endpoint de test accessible uniquement au rôle ChefServiceTIMS
    /// </summary>
    [HttpGet("admin-only")]
    [Authorize(Roles = "ChefServiceTIMS")]
    public IActionResult AdminOnly()
    {
        var timsUserId = HttpContext.Items["TimsUserId"]?.ToString();
        
        return Ok(ApiResponse<object>.Ok(new
        {
            message = "✅ Admin access granted",
            timsUserId
        }));
    }

    /// <summary>
    /// Endpoint public pour tester que l'API fonctionne
    /// </summary>
    [HttpGet("ping")]
    [AllowAnonymous]
    public IActionResult Ping()
    {
        return Ok(ApiResponse<object>.Ok(new
        {
            message = "🏓 TIMS API is running",
            timestamp = DateTime.UtcNow
        }));
    }
}
