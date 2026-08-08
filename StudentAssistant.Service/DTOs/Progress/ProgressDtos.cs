using StudentAssistant.Domain.Enums;

namespace StudentAssistant.Service.DTOs.Progress;

public class UserProgressDto
{
    public long UserId { get; set; }
    public CefrLevel CurrentLevel { get; set; }
    public double OverallProgressPercentage { get; set; }
    public List<LevelProgressDto> LevelProgresses { get; set; } = new();
}

public class LevelProgressDto
{
    public CefrLevel Level { get; set; }
    public double ProgressPercentage { get; set; }
    public int TestsPassed { get; set; }
    public int TestsFailed { get; set; }
    public bool IsUnlocked { get; set; }
}
