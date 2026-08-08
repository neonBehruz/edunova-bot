using Microsoft.AspNetCore.Mvc;
using StudentAssistant.Service.DTOs.Subjects;
using StudentAssistant.Service.Interfaces;
using StudentAssistant.WebApi.Models;

namespace StudentAssistant.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var subjects = await _subjectService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<SubjectDto>>.Ok(subjects));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var subject = await _subjectService.GetByIdAsync(id);
        if (subject == null) return NotFound(ApiResponse<SubjectDto>.Fail("Subject not found"));
        return Ok(ApiResponse<SubjectDto>.Ok(subject));
    }
}
