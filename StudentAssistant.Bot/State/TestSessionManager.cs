#nullable enable

using System.Collections.Concurrent;

namespace StudentAssistant.Bot.State;

public class TestSessionManager
{
    private readonly ConcurrentDictionary<long, TestSession> _activeSessions = new();
    private readonly ConcurrentDictionary<long, UserStateStep> _userStates = new();
    private readonly ConcurrentDictionary<long, (Domain.Enums.CefrLevel? Level, long? SubjectId, Domain.Enums.DifficultyLevel? Difficulty, int? Count)> _userSelections = new();

    public void SetUserState(long userId, UserStateStep step)
    {
        _userStates[userId] = step;
    }

    public UserStateStep GetUserState(long userId)
    {
        return _userStates.TryGetValue(userId, out var step) ? step : UserStateStep.MainMenu;
    }

    public void SetUserLevelSelection(long userId, Domain.Enums.CefrLevel level)
    {
        var existing = _userSelections.TryGetValue(userId, out var val) ? val : (null, null, null, null);
        _userSelections[userId] = (level, existing.SubjectId, existing.Difficulty, existing.Count);
    }

    public void SetUserSubjectSelection(long userId, long subjectId)
    {
        var existing = _userSelections.TryGetValue(userId, out var val) ? val : (null, null, null, null);
        _userSelections[userId] = (existing.Level, subjectId, existing.Difficulty, existing.Count);
    }

    public void SetUserDifficultySelection(long userId, Domain.Enums.DifficultyLevel difficulty)
    {
        var existing = _userSelections.TryGetValue(userId, out var val) ? val : (null, null, null, null);
        _userSelections[userId] = (existing.Level, existing.SubjectId, difficulty, existing.Count);
    }

    public (Domain.Enums.CefrLevel? Level, long? SubjectId, Domain.Enums.DifficultyLevel? Difficulty, int? Count) GetUserSelections(long userId)
    {
        return _userSelections.TryGetValue(userId, out var val) ? val : (null, null, null, null);
    }

    public void StartSession(TestSession session)
    {
        _activeSessions[session.UserId] = session;
    }

    public TestSession? GetSession(long userId)
    {
        return _activeSessions.TryGetValue(userId, out var session) ? session : null;
    }

    public void RemoveSession(long userId)
    {
        _activeSessions.TryRemove(userId, out _);
    }

    public IEnumerable<TestSession> GetAllActiveSessions()
    {
        return _activeSessions.Values;
    }
}
