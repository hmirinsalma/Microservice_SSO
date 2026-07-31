using Microsoft.AspNetCore.Mvc;
using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;

namespace ONEE.SSO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserDto dto)
    {
        var user = await _userService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = user.Id },
            user);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserDto>> Update(Guid id, UpdateUserDto dto)
    {
        var user = await _userService.UpdateAsync(id, dto);

        return Ok(user);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.DeleteAsync(id);

        return NoContent();
    }
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<UserDto>>> Search(string keyword)
    {
        var users = await _userService.SearchAsync(keyword);

        return Ok(users);
    }
    [HttpGet("paged")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetPaged(
    int page = 1,
    int pageSize = 10)
    {
        var users = await _userService.GetPagedAsync(page, pageSize);

        return Ok(users);
    }
    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<UserDto>>> Filter(
    string? firstName,
    string? lastName,
    bool? isActive)
    {
        var users = await _userService.FilterAsync(
            firstName,
            lastName,
            isActive);

        return Ok(users);
    }
    [HttpPut("{id}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _userService.ActivateAsync(id);
        return NoContent();
    }

    [HttpPut("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _userService.DeactivateAsync(id);
        return NoContent();
    }
}