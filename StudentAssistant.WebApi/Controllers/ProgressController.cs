using Microsoft.AspNetCore.Mvc;
using StudentAssistant.Service.DTOs.Progress;
using StudentAssistant.Service.Interfaces;
using StudentAssistant.WebApi.Models;

namespace StudentAssistant.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgressController : ControllerBase
{
    private readonly IProgressService _progressService;

    public ProgressController(IProgressService progressService)
    {
        _progressService = progressService;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserProgress(long userId)
    {
        var progress = await _progressService.GetUserProgressAsync(userId);
        return Ok(ApiResponse<UserProgressDto>.Ok(progress));
    }
}
