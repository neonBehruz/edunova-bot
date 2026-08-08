using Microsoft.EntityFrameworkCore;
using StudentAssistant.Data.Interfaces;
using StudentAssistant.Domain.Entities;
using StudentAssistant.Domain.Enums;
using StudentAssistant.Service.DTOs.Questions;
using StudentAssistant.Service.Interfaces;

namespace StudentAssistant.Service.Services;

public class QuestionService : IQuestionService
{
    private readonly IRepository<Question> _questionRepository;

    public QuestionService(IRepository<Question> questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task<QuestionDto?> GetByIdAsync(long id)
    {
        var question = await _questionRepository.SelectAll(q => q.Id == id)
            .Include(q => q.Options)
            .FirstOrDefaultAsync();

        if (question == null) return null;

        return MapToDto(question);
    }

    public async Task<IEnumerable<QuestionDto>> GetByLevelAndDifficultyAsync(CefrLevel level, DifficultyLevel difficulty)
    {
        var questions = await _questionRepository.SelectAll(q => q.Level == level && q.Difficulty == difficulty)
            .Include(q => q.Options)
            .ToListAsync();

        return questions.Select(MapToDto);
    }

    private static QuestionDto MapToDto(Question q)
    {
        return new QuestionDto
        {
            Id = q.Id,
            SubjectId = q.SubjectId,
            Text = q.Text,
            Level = q.Level,
            Difficulty = q.Difficulty,
            Type = q.Type,
            Explanation = q.Explanation,
            Options = q.Options.Select(o => new AnswerOptionDto
            {
                Id = o.Id,
                QuestionId = o.QuestionId,
                Text = o.Text,
                IsCorrect = o.IsCorrect,
                Order = o.Order
            }).OrderBy(o => o.Order).ToList()
        };
    }
}
