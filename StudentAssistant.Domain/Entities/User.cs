using StudentAssistant.Domain.Common;
using StudentAssistant.Domain.Enums;

namespace StudentAssistant.Domain.Entities;

public class User : Auditable
{
    public long TelegramId { get; set; }
    public string? Username { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public UserRole Role { get; set; } = UserRole.Student;
    public CefrLevel CurrentLevel { get; set; } = CefrLevel.A1;
    public int RatingScore { get; set; } = 0;
    public int TotalTestsTaken { get; set; } = 0;
    public int TotalCorrectAnswers { get; set; } = 0;

    // Navigation properties
    public ICollection<TestAttempt> TestAttempts { get; set; } = new List<TestAttempt>();
    public ICollection<UserProgress> Progresses { get; set; } = new List<UserProgress>();
    public UserRating? Rating { get; set; }
}
