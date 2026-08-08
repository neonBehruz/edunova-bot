using Microsoft.EntityFrameworkCore;
using StudentAssistant.Data.Interfaces;
using StudentAssistant.Domain.Entities;
using StudentAssistant.Domain.Enums;
using StudentAssistant.Service.Interfaces;

namespace StudentAssistant.Service.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<TestAttempt> _attemptRepository;
    private readonly IRepository<StudentAnswer> _answerRepository;

    public StatisticsService(
        IRepository<User> userRepository,
        IRepository<TestAttempt> attemptRepository,
        IRepository<StudentAnswer> answerRepository)
    {
        _userRepository = userRepository;
        _attemptRepository = attemptRepository;
        _answerRepository = answerRepository;
    }

    public async Task<StatisticsOverviewDto> GetOverviewAsync()
    {
        int totalUsers = await _userRepository.SelectAll().CountAsync();
        int totalTests = await _attemptRepository.SelectAll(a => a.Status == TestStatus.Completed).CountAsync();
        int totalAnswers = await _answerRepository.SelectAll().CountAsync();
        double avgScore = 0;

        var completed = await _attemptRepository.SelectAll(a => a.Status == TestStatus.Completed).ToListAsync();
        if (completed.Any())
        {
            avgScore = Math.Round(completed.Average(a => a.ScorePercentage), 1);
        }

        return new StatisticsOverviewDto
        {
            TotalUsers = totalUsers,
            TotalTestsTaken = totalTests,
            TotalQuestionsAnswered = totalAnswers,
            AverageScorePercentage = avgScore
        };
    }
}
