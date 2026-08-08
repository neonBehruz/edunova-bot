using Microsoft.EntityFrameworkCore;
using StudentAssistant.Data.Interfaces;
using StudentAssistant.Domain.Entities;
using StudentAssistant.Domain.Enums;
using StudentAssistant.Service.DTOs.Results;
using StudentAssistant.Service.DTOs.Tests;
using StudentAssistant.Service.Interfaces;

namespace StudentAssistant.Service.Services;

public class TestAttemptService : ITestAttemptService
{
    private readonly IRepository<TestAttempt> _attemptRepository;
    private readonly IRepository<StudentAnswer> _studentAnswerRepository;

    public TestAttemptService(
        IRepository<TestAttempt> attemptRepository,
        IRepository<StudentAnswer> studentAnswerRepository)
    {
        _attemptRepository = attemptRepository;
        _studentAnswerRepository = studentAnswerRepository;
    }

    public async Task<TestAttemptDto> CreateAttemptAsync(StartTestDto startTestDto, List<long> questionIds)
    {
        var attempt = new TestAttempt
        {
            UserId = startTestDto.UserId,
            SubjectId = startTestDto.SubjectId,
            Level = startTestDto.Level,
            Difficulty = startTestDto.Difficulty,
            TotalQuestions = questionIds.Count,
            Status = TestStatus.InProcess,
            StartedAt = DateTime.UtcNow
        };

        var created = await _attemptRepository.AddAsync(attempt);
        await _attemptRepository.SaveChangesAsync();

        return new TestAttemptDto
        {
            Id = created.Id,
            UserId = created.UserId,
            TotalQuestions = created.TotalQuestions,
            StartedAt = created.StartedAt
        };
    }

    public async Task<TestResultDto?> GetAttemptResultAsync(long attemptId)
    {
        var attempt = await _attemptRepository.SelectAll(a => a.Id == attemptId)
            .Include(a => a.StudentAnswers)
                .ThenInclude(sa => sa.Question)
                    .ThenInclude(q => q.Options)
            .Include(a => a.StudentAnswers)
                .ThenInclude(sa => sa.SelectedOption)
            .FirstOrDefaultAsync();

        if (attempt == null) return null;

        var reviews = attempt.StudentAnswers.Select(sa => new QuestionReviewDto
        {
            QuestionText = sa.Question.Text,
            YourAnswer = sa.SelectedOption?.Text ?? "Javob berilmadi",
            CorrectAnswer = sa.Question.Options.FirstOrDefault(o => o.IsCorrect)?.Text ?? "",
            IsCorrect = sa.IsCorrect,
            Explanation = sa.Question.Explanation
        }).ToList();

        return new TestResultDto
        {
            AttemptId = attempt.Id,
            UserId = attempt.UserId,
            Level = attempt.Level,
            Difficulty = attempt.Difficulty,
            TotalQuestions = attempt.TotalQuestions,
            CorrectAnswers = attempt.CorrectAnswers,
            IncorrectAnswers = attempt.TotalQuestions - attempt.CorrectAnswers,
            ScorePercentage = attempt.ScorePercentage,
            DurationSeconds = attempt.DurationSeconds,
            Status = attempt.Status,
            CompletedAt = attempt.CompletedAt ?? DateTime.UtcNow,
            QuestionReviews = reviews
        };
    }

    public async Task<List<ResultHistoryDto>> GetUserHistoryAsync(long userId, int limit = 10)
    {
        var attempts = await _attemptRepository.SelectAll(a => a.UserId == userId && a.Status != TestStatus.InProcess)
            .OrderByDescending(a => a.CompletedAt)
            .Take(limit)
            .ToListAsync();

        return attempts.Select(a => new ResultHistoryDto
        {
            AttemptId = a.Id,
            Level = a.Level,
            Difficulty = a.Difficulty,
            CorrectAnswers = a.CorrectAnswers,
            TotalQuestions = a.TotalQuestions,
            ScorePercentage = a.ScorePercentage,
            CompletedAt = a.CompletedAt ?? a.CreatedAt
        }).ToList();
    }
}
