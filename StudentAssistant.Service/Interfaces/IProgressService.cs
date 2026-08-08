using StudentAssistant.Service.DTOs.Progress;

namespace StudentAssistant.Service.Interfaces;

public interface IProgressService
{
    Task<UserProgressDto> GetUserProgressAsync(long userId);
    Task UpdateProgressAfterTestAsync(long userId, long attemptId);
}
