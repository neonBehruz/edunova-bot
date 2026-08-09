using Microsoft.EntityFrameworkCore;
using StudentAssistant.Data.Interfaces;
using StudentAssistant.Domain.Entities;
using StudentAssistant.Domain.Enums;
using StudentAssistant.Service.Interfaces;

namespace StudentAssistant.Service.Services;

public class QuestionGeneratorService : IQuestionGeneratorService
{
    private readonly IRepository<Question> _questionRepository;
    private readonly IRepository<QuestionHistory> _historyRepository;

    public QuestionGeneratorService(
        IRepository<Question> questionRepository,
        IRepository<QuestionHistory> historyRepository)
    {
        _questionRepository = questionRepository;
        _historyRepository = historyRepository;
    }

    public async Task<List<Question>> GenerateQuestionsAsync(long userId, long subjectId, CefrLevel level, DifficultyLevel difficulty, int count)
    {
        // 1. Fetch candidate questions matching Subject, Level, and Difficulty
        var candidateQuestions = await _questionRepository.SelectAll(q =>
                q.SubjectId == subjectId &&
                q.Level == level &&
                q.Difficulty == difficulty)
            .Include(q => q.Options)
            .ToListAsync();

        if (!candidateQuestions.Any())
        {
            candidateQuestions = await _questionRepository.SelectAll(q =>
                    q.SubjectId == subjectId &&
                    q.Level == level)
                .Include(q => q.Options)
                .ToListAsync();
        }

        if (!candidateQuestions.Any())
        {
            candidateQuestions = await _questionRepository.SelectAll(q => q.SubjectId == subjectId)
                .Include(q => q.Options)
                .ToListAsync();
        }

        // 2. Fetch answered question IDs for THIS specific user from QuestionHistory
        var userAnsweredQuestionIds = await _historyRepository.SelectAll(h => h.UserId == userId && h.TimesAnswered > 0)
            .Select(h => h.QuestionId)
            .ToListAsync();

        // 3. Filter out questions already answered by THIS specific user
        var freshQuestions = candidateQuestions
            .Where(q => !userAnsweredQuestionIds.Contains(q.Id))
            .OrderBy(_ => Guid.NewGuid())
            .Take(count)
            .ToList();

        return freshQuestions;
    }
}
