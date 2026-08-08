using StudentAssistant.Domain.Common;
using StudentAssistant.Domain.Enums;

namespace StudentAssistant.Domain.Entities;

public class UserProgress : Auditable
{
    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public long SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    public CefrLevel Level { get; set; }
    public double ProgressPercentage { get; set; }
    public int TestsPassed { get; set; }
    public int TestsFailed { get; set; }
}
