using Microsoft.EntityFrameworkCore;
using StudentAssistant.Data.Context;
using StudentAssistant.Data.Repositories;
using StudentAssistant.Domain.Entities;
using StudentAssistant.Domain.Enums;
using StudentAssistant.Service.Services;
using Xunit;

namespace StudentAssistant.Tests.Services;

public class QuestionGeneratorTests
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
    public async Task GenerateQuestions_ShouldReturnRequestedCount()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var questionRepo = new Repository<Question>(context);
        var historyRepo = new Repository<QuestionHistory>(context);
        var generator = new QuestionGeneratorService(questionRepo, historyRepo);

        // Act
        var result = await generator.GenerateQuestionsAsync(userId: 1, subjectId: 1, CefrLevel.A1, DifficultyLevel.Easy, count: 3);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.All(result, q => Assert.Equal(CefrLevel.A1, q.Level));
    }
}
