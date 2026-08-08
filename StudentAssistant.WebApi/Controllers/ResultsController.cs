using Microsoft.AspNetCore.Mvc;
using StudentAssistant.Service.DTOs.Results;
using StudentAssistant.Service.Interfaces;
using StudentAssistant.WebApi.Models;

namespace StudentAssistant.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResultsController : ControllerBase
{
    private readonly ITestAttemptService _attemptService;

    public ResultsController(ITestAttemptService attemptService)
    {
        _attemptService = attemptService;
    }

    [HttpGet("attempt/{attemptId}")]
    public async Task<IActionResult> GetAttemptResult(long attemptId)
    {
        var result = await _attemptService.GetAttemptResultAsync(attemptId);
        if (result == null) return NotFound(ApiResponse<TestResultDto>.Fail("Result not found"));
        return Ok(ApiResponse<TestResultDto>.Ok(result));
    }

    [HttpGet("user/{userId}/history")]
    public async Task<IActionResult> GetUserHistory(long userId, [FromQuery] int limit = 10)
    {
        var history = await _attemptService.GetUserHistoryAsync(userId, limit);
        return Ok(ApiResponse<List<ResultHistoryDto>>.Ok(history));
    }
}
