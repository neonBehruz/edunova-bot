using StudentAssistant.Domain.Enums;

namespace StudentAssistant.Service.DTOs.Rating;

public class RatingDto
{
    public int Rank { get; set; }
    public long UserId { get; set; }
    public long TelegramId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? Username { get; set; }
    public CefrLevel Level { get; set; }
    public int RatingScore { get; set; }
    public int TotalTests { get; set; }
}
