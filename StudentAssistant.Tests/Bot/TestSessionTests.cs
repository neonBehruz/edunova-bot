using StudentAssistant.Bot.State;
using StudentAssistant.Domain.Enums;
using Xunit;

namespace StudentAssistant.Tests.Bot;

public class TestSessionTests
{
    [Fact]
    public void TestSessionManager_ShouldStoreAndRetrieveSession()
    {
        // Arrange
        var manager = new TestSessionManager();
        long userId = 1001;

        // Act
        manager.SetUserState(userId, UserStateStep.SelectingDifficulty);
        manager.SetUserLevelSelection(userId, CefrLevel.B2);

        var session = new TestSession
        {
            UserId = userId,
            AttemptId = 55,
            SelectedLevel = CefrLevel.B2,
            SelectedDifficulty = DifficultyLevel.Middle
        };
        manager.StartSession(session);

        // Assert
        Assert.Equal(UserStateStep.SelectingDifficulty, manager.GetUserState(userId));
        Assert.Equal(CefrLevel.B2, manager.GetUserSelections(userId).Level);
        
        var retrievedSession = manager.GetSession(userId);
        Assert.NotNull(retrievedSession);
        Assert.Equal(55, retrievedSession.AttemptId);
        Assert.Equal(DifficultyLevel.Middle, retrievedSession.SelectedDifficulty);
    }
}
