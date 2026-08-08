using StudentAssistant.Domain.Common;

namespace StudentAssistant.Domain.Entities;

public class Subject : Auditable
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
