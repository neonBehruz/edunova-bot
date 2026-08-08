using Microsoft.EntityFrameworkCore;
using StudentAssistant.Data.Interfaces;
using StudentAssistant.Domain.Entities;
using StudentAssistant.Domain.Enums;
using StudentAssistant.Service.DTOs.Results;
using StudentAssistant.Service.DTOs.Tests;
using StudentAssistant.Service.Interfaces;

namespace StudentAssistant.Service.Services;

public class TestService : ITestService
{
    private readonly IRepository<TestAttempt> _attemptRepository;
    private readonly IRepository<StudentAnswer> _studentAnswerRepository;
    private readonly IRepository<Question> _questionRepository;
    private readonly IRepository<AnswerOption> _optionRepository;
    private readonly IRepository<QuestionHistory> _historyRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRatingService _ratingService;
    private readonly IProgressService _progressService;

    public TestService(
        IRepository<TestAttempt> attemptRepository,
        IRepository<StudentAnswer> studentAnswerRepository,
        IRepository<Question> questionRepository,
        IRepository<AnswerOption> optionRepository,
        IRepository<QuestionHistory> historyRepository,
        IRepository<User> userRepository,
        IRatingService ratingService,
        IProgressService progressService)
    {
        _attemptRepository = attemptRepository;
        _studentAnswerRepository = studentAnswerRepository;
        _questionRepository = questionRepository;
        _optionRepository = optionRepository;
        _historyRepository = historyRepository;
        _userRepository = userRepository;
        _ratingService = ratingService;
        _progressService = progressService;
    }

    public async Task<TestResultDto> SubmitTestAsync(long attemptId, List<SubmitAnswerDto> answers)
    {
        var attempt = await _attemptRepository.GetByIdAsync(attemptId);
        if (attempt == null) throw new InvalidOperationException("Test attempt not found.");

        int correctCount = 0;
        int totalDuration = 0;
        var reviews = new List<QuestionReviewDto>();

        foreach (var answerDto in answers)
        {
            totalDuration += answerDto.TimeTakenSeconds;
            var question = await _questionRepository.SelectAll(q => q.Id == answerDto.QuestionId)
                .Include(q => q.Options)
                .FirstOrDefaultAsync();

            if (question == null) continue;

            AnswerOption? selectedOption = null;
            if (answerDto.SelectedOptionId.HasValue)
            {
                selectedOption = question.Options.FirstOrDefault(o => o.Id == answerDto.SelectedOptionId.Value);
            }

            bool isCorrect = selectedOption != null && selectedOption.IsCorrect;
            if (isCorrect) correctCount++;

            var studentAnswer = new StudentAnswer
            {
                TestAttemptId = attemptId,
                QuestionId = answerDto.QuestionId,
                SelectedOptionId = selectedOption?.Id,
                IsCorrect = isCorrect,
                TimeTakenSeconds = answerDto.TimeTakenSeconds
            };
            await _studentAnswerRepository.AddAsync(studentAnswer);

            // Update QuestionHistory
            var history = await _historyRepository.FirstOrDefaultAsync(h => h.UserId == attempt.UserId && h.QuestionId == answerDto.QuestionId);
            if (history == null)
            {
                history = new QuestionHistory
                {
                    UserId = attempt.UserId,
                    QuestionId = answerDto.QuestionId,
                    TimesAnswered = 1,
                    TimesCorrect = isCorrect ? 1 : 0,
                    LastAnsweredAt = DateTime.UtcNow
                };
                await _historyRepository.AddAsync(history);
            }
            else
            {
                history.TimesAnswered += 1;
                if (isCorrect) history.TimesCorrect += 1;
                history.LastAnsweredAt = DateTime.UtcNow;
                _historyRepository.Update(history);
            }

            var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
            reviews.Add(new QuestionReviewDto
            {
                QuestionText = question.Text,
                YourAnswer = selectedOption?.Text ?? "Javob berilmadi",
                CorrectAnswer = correctOption?.Text ?? "",
                IsCorrect = isCorrect,
                Explanation = question.Explanation
            });
        }

        // Calculate score & status
        double percentage = attempt.TotalQuestions > 0 ? (double)correctCount / attempt.TotalQuestions * 100 : 0;
        attempt.CorrectAnswers = correctCount;
        attempt.ScorePercentage = Math.Round(percentage, 2);
        attempt.DurationSeconds = totalDuration;
        attempt.Status = TestStatus.Completed;
        attempt.CompletedAt = DateTime.UtcNow;

        _attemptRepository.Update(attempt);
        await _attemptRepository.SaveChangesAsync();
        await _studentAnswerRepository.SaveChangesAsync();
        await _historyRepository.SaveChangesAsync();

        // Rating calculation formula: (correctCount * 10 * level) + difficultyBonus
        int ratingPoints = (int)(correctCount * 10 * (int)attempt.Level);
        if (percentage >= 80) ratingPoints += 15;

        await _ratingService.UpdateRatingAsync(attempt.UserId, ratingPoints);

        // Update User totals
        var user = await _userRepository.GetByIdAsync(attempt.UserId);
        if (user != null)
        {
            user.TotalTestsTaken += 1;
            user.TotalCorrectAnswers += correctCount;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        // Update User Progress
        await _progressService.UpdateProgressAfterTestAsync(attempt.UserId, attempt.Id);

        return new TestResultDto
        {
            AttemptId = attempt.Id,
            UserId = attempt.UserId,
            Level = attempt.Level,
            Difficulty = attempt.Difficulty,
            TotalQuestions = attempt.TotalQuestions,
            CorrectAnswers = correctCount,
            IncorrectAnswers = attempt.TotalQuestions - correctCount,
            ScorePercentage = attempt.ScorePercentage,
            DurationSeconds = totalDuration,
            RatingPointsEarned = ratingPoints,
            Status = attempt.Status,
            CompletedAt = attempt.CompletedAt.Value,
            QuestionReviews = reviews
        };
    }
}
