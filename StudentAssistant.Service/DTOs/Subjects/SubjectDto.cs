namespace StudentAssistant.Service.DTOs.Subjects;

public class SubjectDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int QuestionCount { get; set; }
}
