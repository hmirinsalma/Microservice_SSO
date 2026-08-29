using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIMS.API.Common;
using TIMS.API.DTOs.User;
using TIMS.API.Entities;
using TIMS.API.Extensions;
using TIMS.API.Interfaces;

namespace TIMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _svc;
    public UsersController(IUserService svc) { _svc = svc; }

    private int UserId => ClaimsHelper.GetTimsUserId(User);

    [HttpGet]
    [Authorize(Roles = RoleNames.AdminTechnique)]
    public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(ApiResponse<PagedResult<UserDto>>.Ok(await _svc.GetAllAsync(page, pageSize)));

    [HttpGet("{id:int}")]
    [Authorize(Roles = RoleNames.AdminTechnique)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetById(int id)
        => Ok(ApiResponse<UserDto>.Ok(await _svc.GetByIdAsync(id)));

    [HttpPost]
    [Authorize(Roles = RoleNames.AdminTechnique)]
    public async Task<ActionResult<ApiResponse<UserDto>>> Create([FromBody] CreateUserDto dto)
    {
        var result = await _svc.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<UserDto>.Ok(result, "Utilisateur créé"));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = RoleNames.AdminTechnique)]
    public async Task<ActionResult<ApiResponse<UserDto>>> Update(int id, [FromBody] UpdateUserDto dto)
        => Ok(ApiResponse<UserDto>.Ok(await _svc.UpdateAsync(id, dto)));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = RoleNames.AdminTechnique)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return Ok(ApiResponse<object>.Ok(null!, "Utilisateur désactivé"));
    }

    // ── Mon Profil ────────────────────────────────────────────────────────────

    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetMyProfile()
        => Ok(ApiResponse<UserDto>.Ok(await _svc.GetByIdAsync(UserId)));

    [HttpPut("me/profile")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateMyProfile([FromBody] UpdateProfileDto dto)
        => Ok(ApiResponse<UserDto>.Ok(await _svc.UpdateProfileAsync(UserId, dto), "Profil mis à jour"));

    [HttpPost("me/photo")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateMyPhoto(IFormFile file)
    {
        await _svc.UpdateProfilePhotoAsync(UserId, file);
        return Ok(ApiResponse<object>.Ok(null!, "Photo mise à jour"));
    }

    // ⚠️ NOTE SSO : Le changement de mot de passe sera géré par le SSO.
    // Cette route est conservée temporairement pour le mode Stub.
    // Elle sera supprimée lors de l'intégration SSO.
    [HttpPost("me/change-password")]
    public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        await _svc.ChangePasswordAsync(UserId, dto);
        return Ok(ApiResponse<object>.Ok(null!, "Mot de passe modifié"));
    }

    [HttpGet("technicians/service/{serviceId:int}")]
    [Authorize(Roles = $"{RoleNames.AdminTechnique},{RoleNames.ChefService}")]
    public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetTechsByService(int serviceId)
        => Ok(ApiResponse<List<UserDto>>.Ok(await _svc.GetTechniciensByServiceAsync(serviceId)));
}
