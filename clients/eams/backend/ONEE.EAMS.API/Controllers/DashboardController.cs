using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONEE.EAMS.Application.Common;
using ONEE.EAMS.Application.Interfaces;

namespace ONEE.EAMS.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _service.GetDashboardAsync(User);
        return Ok(ApiResponse<object>.Ok(result));
    }
}
