using StudentAssistant.Domain.Common;

namespace StudentAssistant.Domain.Entities;

public class UserRating : Auditable
{
    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public int Score { get; set; }
    public int Rank { get; set; }
}
