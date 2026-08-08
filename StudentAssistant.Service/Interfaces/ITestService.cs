using StudentAssistant.Service.DTOs.Results;
using StudentAssistant.Service.DTOs.Tests;

namespace StudentAssistant.Service.Interfaces;

public interface ITestService
{
    Task<TestResultDto> SubmitTestAsync(long attemptId, List<SubmitAnswerDto> answers);
}
