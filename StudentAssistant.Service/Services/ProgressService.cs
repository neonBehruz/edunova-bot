using Microsoft.EntityFrameworkCore;
using StudentAssistant.Data.Interfaces;
using StudentAssistant.Domain.Entities;
using StudentAssistant.Domain.Enums;
using StudentAssistant.Service.DTOs.Progress;
using StudentAssistant.Service.Interfaces;

namespace StudentAssistant.Service.Services;

public class ProgressService : IProgressService
{
    private readonly IRepository<UserProgress> _progressRepository;
    private readonly IRepository<TestAttempt> _attemptRepository;
    private readonly IRepository<User> _userRepository;

    public ProgressService(
        IRepository<UserProgress> progressRepository,
        IRepository<TestAttempt> attemptRepository,
        IRepository<User> userRepository)
    {
        _progressRepository = progressRepository;
        _attemptRepository = attemptRepository;
        _userRepository = userRepository;
    }

    public async Task<UserProgressDto> GetUserProgressAsync(long userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        var currentLevel = user?.CurrentLevel ?? CefrLevel.A1;

        var progresses = await _progressRepository.SelectAll(p => p.UserId == userId)
            .ToListAsync();

        var levels = Enum.GetValues<CefrLevel>();
        var levelProgresses = new List<LevelProgressDto>();

        foreach (var lvl in levels)
        {
            var p = progresses.FirstOrDefault(x => x.Level == lvl);
            bool isUnlocked = lvl <= currentLevel;

            levelProgresses.Add(new LevelProgressDto
            {
                Level = lvl,
                ProgressPercentage = p?.ProgressPercentage ?? 0,
                TestsPassed = p?.TestsPassed ?? 0,
                TestsFailed = p?.TestsFailed ?? 0,
                IsUnlocked = isUnlocked
            });
        }

        double overall = levelProgresses.Average(l => l.ProgressPercentage);

        return new UserProgressDto
        {
            UserId = userId,
            CurrentLevel = currentLevel,
            OverallProgressPercentage = Math.Round(overall, 1),
            LevelProgresses = levelProgresses
        };
    }

    public async Task UpdateProgressAfterTestAsync(long userId, long attemptId)
    {
        var attempt = await _attemptRepository.GetByIdAsync(attemptId);
        if (attempt == null) return;

        bool isNew = false;
        var progress = await _progressRepository.FirstOrDefaultAsync(p => p.UserId == userId && p.Level == attempt.Level);
        if (progress == null)
        {
            isNew = true;
            progress = new UserProgress
            {
                UserId = userId,
                SubjectId = attempt.SubjectId,
                Level = attempt.Level,
                ProgressPercentage = 0,
                TestsPassed = 0,
                TestsFailed = 0
            };
            await _progressRepository.AddAsync(progress);
        }

        bool passed = attempt.ScorePercentage >= 70;
        if (passed)
            progress.TestsPassed += 1;
        else
            progress.TestsFailed += 1;

        // Recalculate average progress for this level
        var userAttemptsForLevel = await _attemptRepository.SelectAll(a => a.UserId == userId && a.Level == attempt.Level && a.Status == TestStatus.Completed)
            .ToListAsync();

        if (userAttemptsForLevel.Any())
        {
            progress.ProgressPercentage = Math.Round(userAttemptsForLevel.Average(a => a.ScorePercentage), 1);
        }

        if (!isNew)
        {
            _progressRepository.Update(progress);
        }
        await _progressRepository.SaveChangesAsync();

        // Level Up Logic: If user passed 2+ tests at current level with >=80% average, promote level!
        var user = await _userRepository.GetByIdAsync(userId);
        if (user != null && attempt.Level == user.CurrentLevel && progress.TestsPassed >= 2 && progress.ProgressPercentage >= 75)
        {
            if ((int)user.CurrentLevel < 6) // C2 max
            {
                user.CurrentLevel = (CefrLevel)((int)user.CurrentLevel + 1);
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();
            }
        }
    }
}
