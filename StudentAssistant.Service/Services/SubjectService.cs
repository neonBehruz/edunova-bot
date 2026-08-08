using Microsoft.EntityFrameworkCore;
using StudentAssistant.Data.Interfaces;
using StudentAssistant.Domain.Entities;
using StudentAssistant.Service.DTOs.Subjects;
using StudentAssistant.Service.Interfaces;

namespace StudentAssistant.Service.Services;

public class SubjectService : ISubjectService
{
    private readonly IRepository<Subject> _subjectRepository;

    public SubjectService(IRepository<Subject> subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<IEnumerable<SubjectDto>> GetAllAsync()
    {
        var subjects = await _subjectRepository.SelectAll()
            .Include(s => s.Questions)
            .ToListAsync();

        return subjects.Select(s => new SubjectDto
        {
            Id = s.Id,
            Name = s.Name,
            Code = s.Code,
            Description = s.Description,
            QuestionCount = s.Questions.Count(q => !q.IsDeleted)
        });
    }

    public async Task<SubjectDto?> GetByIdAsync(long id)
    {
        var subject = await _subjectRepository.SelectAll(s => s.Id == id)
            .Include(s => s.Questions)
            .FirstOrDefaultAsync();

        if (subject == null) return null;

        return new SubjectDto
        {
            Id = subject.Id,
            Name = subject.Name,
            Code = subject.Code,
            Description = subject.Description,
            QuestionCount = subject.Questions.Count(q => !q.IsDeleted)
        };
    }
}
