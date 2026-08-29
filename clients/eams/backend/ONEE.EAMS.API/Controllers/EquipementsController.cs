using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONEE.EAMS.Application.Common;
using ONEE.EAMS.Application.DTOs.Equipement;
using ONEE.EAMS.Application.Interfaces;

namespace ONEE.EAMS.API.Controllers;

[ApiController]
[Route("api/equipements")]
[Authorize]
public class EquipementsController : ControllerBase
{
    private readonly IEquipementService _service;

    public EquipementsController(IEquipementService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] EquipementFilterRequest filter)
    {
        // ⭐ SSO: Récupérer les custom claims EAMS depuis le middleware
        var eamsUserId = HttpContext.Items["EamsUserId"]?.ToString();
        var serviceId = HttpContext.Items["ServiceId"]?.ToString();
        
        // Log pour déboguer (à retirer en production)
        Console.WriteLine($"🔍 SSO Context - EamsUserId: {eamsUserId}, ServiceId: {serviceId}");
        
        var result = await _service.GetAllAsync(filter, User);
        return Ok(ApiResponse<PagedResult<EquipementListDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id, User);
        return Ok(ApiResponse<EquipementDetailDto>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin_Patrimoine")]
    public async Task<IActionResult> Create([FromBody] CreateEquipementRequest request)
    {
        var result = await _service.CreateAsync(request, User);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<EquipementDetailDto>.Ok(result, 201));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin_Patrimoine")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEquipementRequest request)
    {
        var result = await _service.UpdateAsync(id, request, User);
        return Ok(ApiResponse<EquipementDetailDto>.Ok(result));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin_Patrimoine")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id, User);
        return Ok(ApiResponse<object>.Ok(new { message = "Équipement supprimé." }));
    }

    [HttpPatch("{id:guid}/etat")]
    [Authorize(Roles = "Admin_Patrimoine,Chef_de_Service,Technicien")]
    public async Task<IActionResult> UpdateEtat(Guid id, [FromBody] UpdateEtatRequest request)
    {
        var result = await _service.UpdateEtatAsync(id, request, User);
        return Ok(ApiResponse<EquipementDetailDto>.Ok(result));
    }

    [HttpPost("{id:guid}/documents")]
    [Authorize(Roles = "Admin_Patrimoine,Technicien")]
    public async Task<IActionResult> UploadDocument(Guid id, IFormFile file)
    {
        var result = await _service.UploadDocumentAsync(id, file, User);
        return Ok(ApiResponse<DocumentDto>.Ok(result));
    }

    [HttpPost("{id:guid}/photos")]
    [Authorize(Roles = "Admin_Patrimoine,Technicien")]
    public async Task<IActionResult> UploadPhoto(Guid id, IFormFile file)
    {
        var result = await _service.UploadPhotoAsync(id, file, User);
        return Ok(ApiResponse<PhotoDto>.Ok(result));
    }

    [HttpDelete("{id:guid}/documents/{docId:guid}")]
    [Authorize(Roles = "Admin_Patrimoine")]
    public async Task<IActionResult> DeleteDocument(Guid id, Guid docId)
    {
        await _service.DeleteDocumentAsync(id, docId, User);
        return Ok(ApiResponse<object>.Ok(new { message = "Document supprimé." }));
    }

    [HttpGet("{id:guid}/historique")]
    public async Task<IActionResult> GetHistorique(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _service.GetHistoriqueAsync(id, page, pageSize, User);
        return Ok(ApiResponse<PagedResult<Application.DTOs.Historique.HistoriqueEntryDto>>.Ok(result));
    }
}
