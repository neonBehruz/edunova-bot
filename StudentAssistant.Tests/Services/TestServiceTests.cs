using Microsoft.EntityFrameworkCore;
using StudentAssistant.Data.Context;
using StudentAssistant.Data.Repositories;
using StudentAssistant.Domain.Entities;
using StudentAssistant.Domain.Enums;
using StudentAssistant.Service.DTOs.Tests;
using StudentAssistant.Service.Services;
using Xunit;

namespace StudentAssistant.Tests.Services;

public class TestServiceTests
{
    private AppDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task SubmitTest_ShouldCalculateScoreAndPercentageCorrectly()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var userRepo = new Repository<User>(context);
        var attemptRepo = new Repository<TestAttempt>(context);
        var answerRepo = new Repository<StudentAnswer>(context);
        var questionRepo = new Repository<Question>(context);
        var optionRepo = new Repository<AnswerOption>(context);
        var historyRepo = new Repository<QuestionHistory>(context);
        var progressRepo = new Repository<UserProgress>(context);
        var ratingRepo = new Repository<UserRating>(context);

        var user = new User { TelegramId = 123456, FirstName = "TestUser" };
        await userRepo.AddAsync(user);
        await userRepo.SaveChangesAsync();

        var ratingService = new RatingService(userRepo, ratingRepo);
        var progressService = new ProgressService(progressRepo, attemptRepo, userRepo);
        var testService = new TestService(attemptRepo, answerRepo, questionRepo, optionRepo, historyRepo, userRepo, ratingService, progressService);

        var attempt = new TestAttempt
        {
            UserId = user.Id,
            SubjectId = 1,
            Level = CefrLevel.A1,
            Difficulty = DifficultyLevel.Easy,
            TotalQuestions = 1,
            Status = TestStatus.InProcess
        };
        await attemptRepo.AddAsync(attempt);
        await attemptRepo.SaveChangesAsync();

        var question = await questionRepo.SelectAll(q => q.Level == CefrLevel.A1 && q.Difficulty == DifficultyLevel.Easy)
            .Include(q => q.Options)
            .FirstOrDefaultAsync();
        Assert.NotNull(question);

        var correctOption = question.Options.First(o => o.IsCorrect);

        var answers = new List<SubmitAnswerDto>
        {
            new SubmitAnswerDto
            {
                AttemptId = attempt.Id,
                QuestionId = question.Id,
                SelectedOptionId = correctOption.Id,
                TimeTakenSeconds = 15
            }
        };

        // Act
        var result = await testService.SubmitTestAsync(attempt.Id, answers);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.CorrectAnswers);
        Assert.Equal(100, result.ScorePercentage);
        Assert.Equal(TestStatus.Completed, result.Status);
    }
}
