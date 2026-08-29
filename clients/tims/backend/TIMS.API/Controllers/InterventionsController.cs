using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIMS.API.Common;
using TIMS.API.Data;
using TIMS.API.DTOs.Intervention;
using TIMS.API.Entities;
using TIMS.API.Extensions;
using TIMS.API.Interfaces;

namespace TIMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InterventionsController : ControllerBase
{
    private readonly IInterventionService _svc;
    private readonly ApplicationDbContext _db;

    public InterventionsController(IInterventionService svc, ApplicationDbContext db)
    { _svc = svc; _db = db; }

    // Claims résolus via ClaimsHelper (jamais int.Parse direct)
    private int    UserId    => ClaimsHelper.GetTimsUserId(User);
    private string Role      => ClaimsHelper.GetRole(User);
    private int?   ServiceId => ClaimsHelper.GetServiceId(User);

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<InterventionListDto>>>> GetAll([FromQuery] InterventionFilterDto filter)
        => Ok(ApiResponse<PagedResult<InterventionListDto>>.Ok(await _svc.GetAllAsync(filter, UserId, Role, ServiceId)));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<InterventionDto>>> GetById(int id)
        => Ok(ApiResponse<InterventionDto>.Ok(await _svc.GetByIdAsync(id, UserId, Role, ServiceId)));

    [HttpPost]
    [Authorize(Roles = $"{RoleNames.AdminTechnique},{RoleNames.ChefService}")]
    public async Task<ActionResult<ApiResponse<InterventionDto>>> Create([FromBody] CreateInterventionDto dto)
    {
        var result = await _svc.CreateAsync(dto, UserId, Role, ServiceId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<InterventionDto>.Ok(result, "Intervention créée"));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{RoleNames.AdminTechnique},{RoleNames.ChefService}")]
    public async Task<ActionResult<ApiResponse<InterventionDto>>> Update(int id, [FromBody] UpdateInterventionDto dto)
        => Ok(ApiResponse<InterventionDto>.Ok(await _svc.UpdateAsync(id, dto, UserId, Role, ServiceId), "Mise à jour effectuée"));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = RoleNames.AdminTechnique)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _svc.DeleteAsync(id, UserId);
        return Ok(ApiResponse<object>.Ok(null!, "Intervention supprimée"));
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<ApiResponse<InterventionDto>>> ChangeStatus(int id, [FromBody] ChangeStatusDto dto)
        => Ok(ApiResponse<InterventionDto>.Ok(await _svc.ChangeStatusAsync(id, dto, UserId, Role)));

    [HttpPatch("{id:int}/priority")]
    [Authorize(Roles = $"{RoleNames.AdminTechnique},{RoleNames.ChefService}")]
    public async Task<ActionResult<ApiResponse<InterventionDto>>> ChangePriority(int id, [FromBody] ChangePriorityDto dto)
        => Ok(ApiResponse<InterventionDto>.Ok(await _svc.ChangePriorityAsync(id, dto, UserId, Role)));

    [HttpPatch("{id:int}/assign")]
    [Authorize(Roles = $"{RoleNames.AdminTechnique},{RoleNames.ChefService}")]
    public async Task<ActionResult<ApiResponse<InterventionDto>>> Assign(int id, [FromBody] AssignTechnicienDto dto)
        => Ok(ApiResponse<InterventionDto>.Ok(await _svc.AssignTechnicienAsync(id, dto, UserId, Role, ServiceId)));

    [HttpPost("{id:int}/comments")]
    [Authorize(Roles = $"{RoleNames.AdminTechnique},{RoleNames.ChefService},{RoleNames.Technicien}")]
    public async Task<ActionResult<ApiResponse<CommentDto>>> AddComment(int id, [FromBody] AddCommentDto dto)
        => Ok(ApiResponse<CommentDto>.Ok(await _svc.AddCommentAsync(id, dto, UserId, Role)));

    [HttpPatch("{id:int}/compte-rendu")]
    [Authorize(Roles = RoleNames.Technicien)]
    public async Task<ActionResult<ApiResponse<InterventionDto>>> UpdateCR(int id, [FromBody] UpdateCompteRenduDto dto)
        => Ok(ApiResponse<InterventionDto>.Ok(await _svc.UpdateCompteRenduAsync(id, dto, UserId)));

    [HttpGet("{id:int}/history")]
    public async Task<ActionResult<ApiResponse<List<HistoryDto>>>> GetHistory(int id)
        => Ok(ApiResponse<List<HistoryDto>>.Ok(await _svc.GetHistoryAsync(id, UserId, Role, ServiceId)));

    [HttpPost("{id:int}/attachments")]
    public async Task<ActionResult<ApiResponse<AttachmentDto>>> AddAttachment(int id, IFormFile file)
        => Ok(ApiResponse<AttachmentDto>.Ok(await _svc.AddAttachmentAsync(id, file, UserId, Role)));

    [HttpDelete("attachments/{aid:int}")]
    [Authorize(Roles = RoleNames.AdminTechnique)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteAttachment(int aid)
    {
        await _svc.DeleteAttachmentAsync(aid, UserId);
        return Ok(ApiResponse<object>.Ok(null!, "Pièce jointe supprimée"));
    }

    [HttpGet("attachments/{aid:int}/download")]
    public async Task<IActionResult> Download(int aid)
    {
        var att = await _db.Attachments.FindAsync(aid);
        if (att == null) return NotFound();
        var path = Path.Combine("Uploads", "attachments", att.StoredFileName);
        if (!System.IO.File.Exists(path)) return NotFound();
        return PhysicalFile(Path.GetFullPath(path), att.ContentType, att.OriginalFileName);
    }
}
