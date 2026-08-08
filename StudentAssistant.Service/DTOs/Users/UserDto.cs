using StudentAssistant.Domain.Enums;

namespace StudentAssistant.Service.DTOs.Users;

public class UserDto
{
    public long Id { get; set; }
    public long TelegramId { get; set; }
    public string? Username { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public UserRole Role { get; set; }
    public CefrLevel CurrentLevel { get; set; }
    public int RatingScore { get; set; }
    public int TotalTestsTaken { get; set; }
    public int TotalCorrectAnswers { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateUserDto
{
    public long TelegramId { get; set; }
    public string? Username { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public UserRole Role { get; set; } = UserRole.Student;
}
