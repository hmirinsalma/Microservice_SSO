using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONEE.EAMS.Application.Common;
using ONEE.EAMS.Application.DTOs.Categorie;
using ONEE.EAMS.Application.Interfaces;

namespace ONEE.EAMS.API.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategorieService _service;

    public CategoriesController(ICategorieService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<CategorieDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(ApiResponse<CategorieDto>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin_Patrimoine")]
    public async Task<IActionResult> Create([FromBody] CreateCategorieRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<CategorieDto>.Ok(result, 201));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin_Patrimoine")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategorieRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        return Ok(ApiResponse<CategorieDto>.Ok(result));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin_Patrimoine")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return Ok(ApiResponse<object>.Ok(new { message = "Catégorie supprimée." }));
    }
}
