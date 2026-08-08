using StudentAssistant.Service.DTOs.Results;
using StudentAssistant.Service.DTOs.Tests;

namespace StudentAssistant.Service.Interfaces;

public interface ITestAttemptService
{
    Task<TestAttemptDto> CreateAttemptAsync(StartTestDto startTestDto, List<long> questionIds);
    Task<TestResultDto?> GetAttemptResultAsync(long attemptId);
    Task<List<ResultHistoryDto>> GetUserHistoryAsync(long userId, int limit = 10);
}

public class TestAttemptDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public int TotalQuestions { get; set; }
    public DateTime StartedAt { get; set; }
}
