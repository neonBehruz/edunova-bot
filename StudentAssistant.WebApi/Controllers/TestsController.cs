using Microsoft.AspNetCore.Mvc;
using StudentAssistant.Service.DTOs.Results;
using StudentAssistant.Service.DTOs.Tests;
using StudentAssistant.Service.Interfaces;
using StudentAssistant.WebApi.Models;

namespace StudentAssistant.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestsController : ControllerBase
{
    private readonly IQuestionGeneratorService _generatorService;
    private readonly ITestAttemptService _attemptService;
    private readonly ITestService _testService;

    public TestsController(
        IQuestionGeneratorService generatorService,
        ITestAttemptService attemptService,
        ITestService testService)
    {
        _generatorService = generatorService;
        _attemptService = attemptService;
        _testService = testService;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartTest([FromBody] StartTestDto dto)
    {
        var questions = await _generatorService.GenerateQuestionsAsync(dto.UserId, dto.SubjectId, dto.Level, dto.Difficulty, dto.QuestionCount);
        if (!questions.Any()) return BadRequest(ApiResponse<TestAttemptDto>.Fail("No questions available for requested criteria"));

        var attempt = await _attemptService.CreateAttemptAsync(dto, questions.Select(q => q.Id).ToList());
        return Ok(ApiResponse<TestAttemptDto>.Ok(attempt, "Test started successfully"));
    }

    [HttpPost("submit/{attemptId}")]
    public async Task<IActionResult> SubmitTest(long attemptId, [FromBody] List<SubmitAnswerDto> answers)
    {
        var result = await _testService.SubmitTestAsync(attemptId, answers);
        return Ok(ApiResponse<TestResultDto>.Ok(result, "Test submitted successfully"));
    }
}
