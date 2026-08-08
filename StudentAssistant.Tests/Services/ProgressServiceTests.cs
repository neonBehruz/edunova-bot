using Microsoft.EntityFrameworkCore;
using StudentAssistant.Data.Context;
using StudentAssistant.Data.Repositories;
using StudentAssistant.Domain.Entities;
using StudentAssistant.Domain.Enums;
using StudentAssistant.Service.Services;
using Xunit;

namespace StudentAssistant.Tests.Services;

public class ProgressServiceTests
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
    public async Task GetUserProgress_ShouldReturnAllCefrLevels()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var userRepo = new Repository<User>(context);
        var progressRepo = new Repository<UserProgress>(context);
        var attemptRepo = new Repository<TestAttempt>(context);

        var user = new User { TelegramId = 777, FirstName = "StudentProgress", CurrentLevel = CefrLevel.A2 };
        await userRepo.AddAsync(user);
        await userRepo.SaveChangesAsync();

        var progressService = new ProgressService(progressRepo, attemptRepo, userRepo);

        // Act
        var result = await progressService.GetUserProgressAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(CefrLevel.A2, result.CurrentLevel);
        Assert.Equal(6, result.LevelProgresses.Count); // A1, A2, B1, B2, C1, C2
        Assert.True(result.LevelProgresses.First(l => l.Level == CefrLevel.A1).IsUnlocked);
        Assert.True(result.LevelProgresses.First(l => l.Level == CefrLevel.A2).IsUnlocked);
        Assert.False(result.LevelProgresses.First(l => l.Level == CefrLevel.B1).IsUnlocked);
    }
}
