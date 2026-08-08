using Microsoft.AspNetCore.Mvc;
using StudentAssistant.Domain.Enums;
using StudentAssistant.Service.DTOs.Questions;
using StudentAssistant.Service.Interfaces;
using StudentAssistant.WebApi.Models;

namespace StudentAssistant.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionsController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var question = await _questionService.GetByIdAsync(id);
        if (question == null) return NotFound(ApiResponse<QuestionDto>.Fail("Question not found"));
        return Ok(ApiResponse<QuestionDto>.Ok(question));
    }

    [HttpGet("level/{level}/difficulty/{difficulty}")]
    public async Task<IActionResult> GetByLevelAndDifficulty(CefrLevel level, DifficultyLevel difficulty)
    {
        var questions = await _questionService.GetByLevelAndDifficultyAsync(level, difficulty);
        return Ok(ApiResponse<IEnumerable<QuestionDto>>.Ok(questions));
    }
}
