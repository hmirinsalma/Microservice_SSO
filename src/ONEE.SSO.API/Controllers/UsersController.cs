using Microsoft.AspNetCore.Mvc;
using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ONEE.SSO.Application.Features.Users.Commands;
using ONEE.SSO.Application.Features.Users.Handlers;

namespace ONEE.SSO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly UnlockUserCommandHandler _unlockUserCommandHandler;

    public UsersController(IUserService userService, UnlockUserCommandHandler unlockUserCommandHandler)
    {
        _userService = userService;
        _unlockUserCommandHandler = unlockUserCommandHandler;
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

    /// <summary>
    /// Débloquer un compte utilisateur verrouillé (Admin uniquement)
    /// </summary>
    [HttpPost("{id:guid}/unlock")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Unlock(Guid id)
    {
        var adminUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(adminUserIdClaim) || !Guid.TryParse(adminUserIdClaim, out var adminUserId))
        {
            return Unauthorized(new { message = "Token invalide" });
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var command = new UnlockUserCommand
        {
            UserId = id,
            AdminUserId = adminUserId,
            IpAddress = ipAddress
        };

        var result = await _unlockUserCommandHandler.HandleAsync(command);

        if (!result)
        {
            return NotFound(new { message = "Utilisateur introuvable" });
        }

        return Ok(new { message = "Compte débloqué avec succès" });
    }
}