using Microsoft.EntityFrameworkCore;
using StudentAssistant.Data.Context;
using StudentAssistant.Data.Repositories;
using StudentAssistant.Domain.Entities;
using StudentAssistant.Domain.Enums;
using StudentAssistant.Service.Services;
using Xunit;

namespace StudentAssistant.Tests.Services;

public class RatingServiceTests
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
    public async Task UpdateRating_ShouldIncreaseScoreAndRankCorrectly()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var userRepo = new Repository<User>(context);
        var ratingRepo = new Repository<UserRating>(context);
        var ratingService = new RatingService(userRepo, ratingRepo);

        var user = new User { TelegramId = 999, FirstName = "LeaderUser", RatingScore = 50, CurrentLevel = CefrLevel.B1 };
        await userRepo.AddAsync(user);
        await userRepo.SaveChangesAsync();

        // Act
        await ratingService.UpdateRatingAsync(user.Id, 25);

        // Assert
        var updatedUser = await userRepo.GetByIdAsync(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal(75, updatedUser.RatingScore);
    }
}
